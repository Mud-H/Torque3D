//-----------------------------------------------------------------------------
// Copyright (c) 2014 R.G.S. - Richards Game Studio, the Netherlands
//					  http://www.richardsgamestudio.com/
// All rights reserved.
// R.G.S. END USER LICENSE AGREEMENT
// =================================
// 
// THIS EULA APPLIES ON THE SOURCE CODE, HEREAFTER CALLED
// CODE.
// 
// YOUR USE OF THIS CODE MEANS THAT YOU ACCEPT THESE TERMS AND CONDITIONS.
// IF YOU DO NOT ACCEPT THESE TERMS AND CONDITIONS, YOU SHOULD NOT USE THIS
// CODE.
// 
// License Terms:
// --------------
// 
// * 1. SINGLE COPY SOFTWARE LICENSE:
// The code is copyrighted and protected by law and international treaty.
// You are allowed to make derivative works based on this code for use in 
// your commercial or non-commercial projects as long as you do not sell or
// transfer this modified code without written permission of R.G.S. You may 
// use the code in any application you are developing, including game
// development in commercial or non-commercial projects. Reasonable
// efforts to encrypt content would be appreciated, but if it is not possible
// accompanying documentation should inform the end user of the copyright
// restrictions on the code included in the released software. No other use
// is granted unless specifically licensed to do otherwise by R.G.S. This is
// a license, not a transfer of title, and you may not:
// 
// A) create derivative works based on this code for retail sale or
//    distribution by any means other than as provided by this agreement.
// B) transfer this code or the documentation in whole or in part to another
//    person without R.G.S.'s written permission.
// C) remove any copyright or other notices. You may not permit anyone else to
//    modify this code included for any commercial purpose.
// D) transfer this code to another person. You agree to prevent any copying
//    of this code to any other person.
// 
// * 2. OWNERSHIP:
// This code is proprietary material of R.G.S., which you have licensed for
// use and except as provided for by this agreement, may not be copied,
// reproduced, published, uploaded, posted, transmitted, or distributed in
// any way, without R.G.S.'s prior written permission.
// 
// * 3. TERMINATION OF THIS LICENSE:
// R.G.S. may terminate this license at any time if you are in breach of the
// terms and conditions of use. Upon such termination you must and agree to
// immediately destroy all copies of this code and all of its accompanying
// documentation.
// 
// * 4. DISCLAIMER AND WARRANTY:
// R.G.S. provides the products related to this agreement "AS IS" without
// warranty of any kind.
// 
// * 5. OTHER:
// In case of doubt about using this code, please contact R.G.S. at
// the following e-mail address: support@richardsgamestudio.com
// 
// September 2015, R.G.S. - Richards Game Studio, Nieuwerkerk a/d IJssel, NL
//-----------------------------------------------------------------------------

#include "environment/skyLine.h"
#include "materials/shaderData.h"
#include "gfx/gfxTransformSaver.h"
#include "gfx/gfxTextureManager.h"
#include "core/stream/bitStream.h"
#include "materials/sceneData.h"
#include "renderInstance/renderBinManager.h"
#include "gfx/gfxDebugEvent.h"
#include "math/util/matrixSet.h"
#include "windowManager/platformWindowMgr.h"
#include "gui/core/GUICanvas.h"
#include "console/engineAPI.h"

IMPLEMENT_CO_NETOBJECT_V1(SkyLine);

ConsoleDocClass(SkyLine,
	"@brief Represents the skyline with an artist-created cubemap.\n\n"

	"SkyLine is a cubemap representing a skyline and to be used in conjunction with a ScatterSky object.\n"
	"The cubemap contains an alpha channel, whereas an alpha value lower than 1.0 (or 255) will be combined with "
	"the scattersky object.\n"

	"@ingroup Environment"
	);

GFXImplementVertexFormat(GFXSkyVertex)
{
	addElement("POSITION", GFXDeclType_Float3);
	addElement("NORMAL", GFXDeclType_Float3);
	addElement("TEXCOORD", GFXDeclType_Float2, 0);
	addElement("TEXCOORD", GFXDeclType_Float, 1);
}

SkyLine::SkyLine()
{
	mTypeMask |= EnvironmentObjectType | StaticObjectType;
	mNetFlags.set(Ghostable | ScopeAlways);

	mSkyLineCubemap = NULL;
	mSkyLineCubemapName = "";

	mIsVBDirty = false;
	mDrawBottom = true;
	mPrimCount = 0;
	mFogBandEnd = 0;

	mMatrixSet = reinterpret_cast<MatrixSet *>(dMalloc_aligned(sizeof(MatrixSet), 16));
	constructInPlace(mMatrixSet);

	mShader = NULL;
	mMaskShader = NULL;
	mCreateMask = false;
	mOldMask = mCreateMask;

	mMaskBuffer = NULL;
	mMaskInit = false;
	mPlatformWindow = NULL;
	z_buf = NULL;
}

SkyLine::~SkyLine()
{
	Con::setBoolVariable("$LightRayPostFX::useMask", false);

	if (mMaskInit)
	{
		if (mMaskTarget.isRegistered())
			GFXTextureManager::removeEventDelegate(this, &SkyLine::_onTextureEvent);
	}

	dFree_aligned(mMatrixSet);

	if (isClientObject())
		return;

	if (z_buf.isValid())
		SAFE_DELETE(z_buf);

	if (mMaskTarget.isRegistered())
		mMaskTarget.unregister();

	if (mMaskBuffer.isValid())
		mMaskBuffer->kill();
}

bool SkyLine::onAdd()
{
	if (!Parent::onAdd())
		return false;

	setGlobalBounds();
	resetWorldBox();

	addToScene();

	if (isClientObject())
	{
		GuiCanvas* cv = dynamic_cast<GuiCanvas*>(Sim::findObject("Canvas"));
		if (cv == NULL)
		{
			Con::errorf("SkyLine::onAdd() - Canvas not found!!");
			return false;
		}
		mPlatformWindow = cv->getPlatformWindow();
		if (mPlatformWindow == NULL)
		{
			Con::errorf("SkyLine::onAdd() - PlatformWindow not found!!");
			return false;
		}

		_initMeshes();
		_updateMaterial();
		_setupRender();
		Con::setBoolVariable("$LightRayPostFX::useMask", _updateMask());
	}

	return true;
}

void SkyLine::onRemove()
{
	removeFromScene();
	Parent::onRemove();
}

void SkyLine::initPersistFields()
{
	addGroup("Sky Line");

	addField("SkyLine", TypeCubemapName, Offset(mSkyLineCubemapName, SkyLine),
		"The name of a cubemap material for the sky box.");

	addField("drawBottom", TypeBool, Offset(mDrawBottom, SkyLine),
		"If false the bottom of the SkyLine is not rendered.");

	addField("fogBandTop", TypeF32, Offset(mFogBandEnd, SkyLine),
		"The height (0-1.0) of the fog band from the horizon to the top of the SkyLine.");

	addField("createMask", TypeBool, Offset(mCreateMask, SkyLine), "If enabled then a mask will be generated for use with the Lightray PostFX");

	endGroup("Sky Line");

	Parent::initPersistFields();
}

void SkyLine::inspectPostApply()
{
	Parent::inspectPostApply();
	_updateMaterial();
	setMaskBits(skyLineMask | maskMask);
}

U32 SkyLine::packUpdate(NetConnection *conn, U32 mask, BitStream *stream)
{
	U32 retMask = Parent::packUpdate(conn, mask, stream);

	if (stream->writeFlag(mask & maskMask))
		stream->writeFlag(mCreateMask);
	if (stream->writeFlag(mask & skyLineMask))
	{
		stream->write(mSkyLineCubemapName);
		stream->writeFlag(mDrawBottom);
		stream->write(mFogBandEnd / 2.0f);
	}

	return retMask;
}

void SkyLine::unpackUpdate(NetConnection *conn, BitStream *stream)
{
	Parent::unpackUpdate(conn, stream);

	bool maskUpdate = stream->readFlag();
	bool newMask = false;

	if (maskUpdate)
		newMask = stream->readFlag();

	if (stream->readFlag())
	{
		String tmpString("");
		stream->read(&tmpString);

		bool drawBottom = stream->readFlag();

		stream->read(&mFogBandEnd);
		if (!tmpString.equal(mSkyLineCubemapName, String::NoCase))
		{
			mSkyLineCubemapName = tmpString;
			_updateMaterial();
		}

		if (drawBottom != mDrawBottom)
		{
			mDrawBottom = drawBottom;
			mIsVBDirty = true;
			_initMeshes();
		}
	}
	if (maskUpdate)
	{
		if (newMask != mCreateMask)
		{
			mCreateMask = newMask;
			Con::setBoolVariable("$LightRayPostFX::useMask",_updateMask());
		}
	}
}

void SkyLine::prepRenderImage(SceneRenderState *state)
{
	PROFILE_SCOPE(SkyLine_prepRenderImage);

	if (state->isShadowPass() ||
		mVB.isNull() ||
		!mSkyLineCubemap || !mShader)
		return;

	mMatrixSet->setSceneView(GFX->getWorldMatrix());
	mMatrixSet->setSceneProjection(GFX->getProjectionMatrix());
	RenderPassManager* passMan = state->getRenderPass();
	ObjectRenderInst *ri = passMan->allocInst<ObjectRenderInst>();
	ri->renderDelegate.bind(this, &SkyLine::_renderObject);
	ri->type = RenderPassManager::RIT_Sky;
	ri->defaultKey = 10;
	ri->defaultKey2 = 3;
	passMan->addInst(ri);
}

void SkyLine::_renderObject(ObjectRenderInst *ri, SceneRenderState *state, BaseMatInstance *mi)
{
	GFXDEBUGEVENT_SCOPE(SkyLine_RenderObject, ColorF::WHITE);

	GFXTransformSaver saver;

	const Point3F camPos = state->getCameraPosition();
	ColorF fogColor = state->getSceneManager()->getFogData().color;

	MatrixF xfm(true);
	xfm.setPosition(camPos);
	GFX->multWorld(xfm);

	if (state->isReflectPass())
		GFX->setProjectionMatrix(state->getSceneManager()->getNonClipProjection());

	GFX->setShader(mShader);
	GFX->setShaderConstBuffer(mShaderConsts);
	GFX->setStateBlockByDesc(mSBDesc);

	MatrixF xform(GFX->getProjectionMatrix());
	xform *= GFX->getViewMatrix();
	xform *= GFX->getWorldMatrix();

	GFX->setVertexBuffer(mVB);

	if (!mSkyLineCubemap->mCubemap)
		mSkyLineCubemap->createMap();
	GFX->setCubeTexture(0, mSkyLineCubemap->mCubemap);

	// Set all the shader consts...

	mShaderConsts->setSafe(mModelViewProjSC, xform);
	mShaderConsts->setSafe(mObjTransSC, xfm);
	mShaderConsts->setSafe(mEyePosSC, camPos);
	mShaderConsts->setSafe(mAmbientColorSC, state->getAmbientLightColor());
	mShaderConsts->setSafe(mHazeColorSC, fogColor);
	mShaderConsts->setSafe(mHazeDataSC, mFogBandEnd);
	mShaderConsts->setSafe(mHDRTypeSC, Con::getFloatVariable("$shaders::HDRType"));// ME Fix precomp shaders

	GFX->drawPrimitive(GFXTriangleList, 0, mPrimCount);

	if (mCreateMask && mMaskInit && state->isDiffusePass())
	{
		GFX->setShader(mMaskShader);
		GFX->setShaderConstBuffer(mMaskShaderConsts);

		mMaskShaderConsts->setSafe(mMaskModelViewProjSC, xform);
		mMaskShaderConsts->setSafe(mMaskObjTransSC, xfm);
		mMaskShaderConsts->setSafe(mMaskEyePosSC, camPos);
		mMaskShaderConsts->setSafe(mAlphaSC, Con::getFloatVariable("$SkyLineMask::bias"));

		GFX->pushActiveRenderTarget();

		z_buf->attachTexture(GFXTextureTarget::Color0, mMaskBuffer);

		GFX->setActiveRenderTarget(z_buf);
		GFX->clear(GFXClearTarget, ColorI(255, 255, 255, 255), 1.0f, 0);

		GFX->drawPrimitive(GFXTriangleList, 0, mPrimCount);
		z_buf->resolve();
		GFX->popActiveRenderTarget();
		z_buf->attachTexture(GFXTextureTarget::Color0, NULL);
	}
}

void SkyLine::_initMeshes()
{
	GFXSkyVertex *tmpVerts = NULL;

	U32 vertCount = 30;

	if (!mDrawBottom)
		vertCount = 24;

	mPrimCount = vertCount / 3;

	tmpVerts = new GFXSkyVertex[vertCount];

	tmpVerts[0].point.set(-1, -1, 1);
	tmpVerts[1].point.set(1, -1, 1);
	tmpVerts[2].point.set(1, -1, -1);

	tmpVerts[0].texCoord.set(0, 0);
	tmpVerts[1].texCoord.set(1.0f, 0);
	tmpVerts[2].texCoord.set(1.0f, 1.0f);

	tmpVerts[3].point.set(-1, -1, 1);
	tmpVerts[4].point.set(1, -1, -1);
	tmpVerts[5].point.set(-1, -1, -1);

	tmpVerts[3].texCoord.set(0, 0);
	tmpVerts[4].texCoord.set(1.0f, 1.0f);
	tmpVerts[5].texCoord.set(0, 1.0f);

	tmpVerts[6].point.set(1, -1, 1);
	tmpVerts[7].point.set(1, 1, 1);
	tmpVerts[8].point.set(1, 1, -1);

	tmpVerts[6].texCoord.set(0, 0);
	tmpVerts[7].texCoord.set(1.0f, 0);
	tmpVerts[8].texCoord.set(1.0f, 1.0f);

	tmpVerts[9].point.set(1, -1, 1);
	tmpVerts[10].point.set(1, 1, -1);
	tmpVerts[11].point.set(1, -1, -1);

	tmpVerts[9].texCoord.set(0, 0);
	tmpVerts[10].texCoord.set(1.0f, 1.0f);
	tmpVerts[11].texCoord.set(0, 1.0f);

	tmpVerts[12].point.set(-1, 1, 1);
	tmpVerts[13].point.set(-1, -1, 1);
	tmpVerts[14].point.set(-1, -1, -1);

	tmpVerts[12].texCoord.set(0, 0);
	tmpVerts[13].texCoord.set(1.0f, 0);
	tmpVerts[14].texCoord.set(1.0f, 1.0f);

	tmpVerts[15].point.set(-1, 1, 1);
	tmpVerts[16].point.set(-1, -1, -1);
	tmpVerts[17].point.set(-1, 1, -1);

	tmpVerts[15].texCoord.set(0, 0);
	tmpVerts[16].texCoord.set(1.0f, 1.0f);
	tmpVerts[17].texCoord.set(1.0f, 1.0f);

	tmpVerts[18].point.set(1, 1, 1);
	tmpVerts[19].point.set(-1, 1, 1);
	tmpVerts[20].point.set(-1, 1, -1);

	tmpVerts[18].texCoord.set(0, 0);
	tmpVerts[19].texCoord.set(1.0f, 0);
	tmpVerts[20].texCoord.set(1.0f, 1.0f);

	tmpVerts[21].point.set(1, 1, 1);
	tmpVerts[22].point.set(-1, 1, -1);
	tmpVerts[23].point.set(1, 1, -1);

	tmpVerts[21].texCoord.set(0, 0);
	tmpVerts[22].texCoord.set(1.0f, 1.0f);
	tmpVerts[23].texCoord.set(0, 1.0f);

	if (mDrawBottom)
	{
		tmpVerts[24].point.set(1, 1, -1);
		tmpVerts[25].point.set(-1, 1, -1);
		tmpVerts[26].point.set(-1, -1, -1);

		tmpVerts[24].texCoord.set(1.0f, 1.0f);
		tmpVerts[25].texCoord.set(1.0f, 0);
		tmpVerts[26].texCoord.set(0, 0);

		tmpVerts[27].point.set(1, -1, -1);
		tmpVerts[28].point.set(1, 1, -1);
		tmpVerts[29].point.set(-1, -1, -1);

		tmpVerts[27].texCoord.set(0, 1.0f);
		tmpVerts[28].texCoord.set(1.0f, 1.0f);
		tmpVerts[29].texCoord.set(0, 0);
	}

	VectorF tmp(0, 0, 0);

	for (U32 i = 0; i < vertCount; i++)
	{
		tmpVerts[i].normal.set(Point3F::Zero);
		if (i<24)
			tmpVerts[i].up = 0.0;
		else
			tmpVerts[i].up = 1.0;
	}

	if (mVB.isNull() || mIsVBDirty)
	{
		mVB.set(GFX, vertCount, GFXBufferTypeStatic);
		mIsVBDirty = false;
	}

	GFXSkyVertex *vertPtr = mVB.lock();
	if (!vertPtr)
	{
		delete[] tmpVerts;
		return;
	}

	dMemcpy(vertPtr, tmpVerts, sizeof (GFXSkyVertex)* vertCount);

	mVB.unlock();

	// Clean up temp verts.
	delete[] tmpVerts;

}

bool SkyLine::_setupRender()
{
	ShaderData *shaderData;
	mShader = Sim::findObject("SkyLineShader", shaderData) ?
		shaderData->getShader() : NULL;
	if (!mShader)
	{
		Con::errorf("SkyLine::_setupRender() - could not find SkyLineShader");
		return false;
	}

	// Create ShaderConstBuffer and Handles

	mShaderConsts = mShader->allocConstBuffer();
	if (mShaderConsts.isNull())
	{
		Con::errorf("SkyLine::_initMaterial() - could not allocate ShaderConstants 1.");
		return false;
	}

	mModelViewProjSC = mShader->getShaderConstHandle("$modelView");
	mObjTransSC = mShader->getShaderConstHandle("$objTrans");
	mEyePosSC = mShader->getShaderConstHandle("$eyePosWorld");
	mAmbientColorSC = mShader->getShaderConstHandle("$ambientColor");
	mHazeColorSC = mShader->getShaderConstHandle("$fogColor");
	mHazeDataSC = mShader->getShaderConstHandle("$fogHeight");
	mHDRTypeSC = mShader->getShaderConstHandle("$HDRType");// ME Fix precomp shaders

	// We want to disable culling and z write.
	mSBDesc.setCullMode(GFXCullCW);
	mSBDesc.setZReadWrite(true, false);
	mSBDesc.setBlend(true);
	mSBDesc.stencilEnable = false;

	mSBDesc.samplers[0].addressModeU = GFXAddressClamp;
	mSBDesc.samplers[0].addressModeV = GFXAddressClamp;
	mSBDesc.samplers[0].addressModeW = GFXAddressClamp;
	mSBDesc.samplers[0].magFilter = GFXTextureFilterLinear;
	mSBDesc.samplers[0].minFilter = GFXTextureFilterLinear;
	mSBDesc.samplers[0].mipFilter = GFXTextureFilterLinear;
	mSBDesc.samplers[0].textureColorOp = GFXTOPModulate;

	return true;
}

void SkyLine::_updateMaterial()
{
	if (mSkyLineCubemapName.isEmpty())
		return;

	CubemapData *pMat = NULL;
	if (!Sim::findObject(mSkyLineCubemapName, pMat))
		Con::printf("SkyLine::_updateMaterial, failed to find cubemap of name %s!", mSkyLineCubemapName.c_str());
	else if (isProperlyAdded())
		mSkyLineCubemap = pMat;
}

void SkyLine::_onTextureEvent(GFXTexCallbackCode code)
{
	if (!isClientObject())
		return;
	if (code == GFXZombify)
	{
		Con::setBoolVariable("$LightRayPostFX::useMask", false);
		mMaskInit = false;
		if (mMaskTarget.isRegistered())
			mMaskTarget.unregister();

		if (mMaskBuffer.isValid())
		{
			mMaskBuffer->kill();
			mMaskBuffer = NULL;
		}
	}
	else
	{
		GFXTextureManager::removeEventDelegate(this, &SkyLine::_onTextureEvent);
		Con::setBoolVariable("$LightRayPostFX::useMask", _updateMask());
	}
}

bool SkyLine::_updateMask()
{
	if (mCreateMask)
	{
		if (mPlatformWindow == NULL)
			return false;
		U32 mWidth = mFloor(mPlatformWindow->getClientExtent().x);
		U32 mHeight = mPlatformWindow->getClientExtent().y;

		if (!mMaskBuffer.isValid() || mMaskBuffer.isNull())
		{
			mMaskBuffer = GFXTexHandle(mWidth, mHeight, GFXFormatR8G8B8A8,
				&GFXDefaultRenderTargetProfile, avar("%s() - mMaskBuffer (line %d)", __FUNCTION__, __LINE__));
			if (!mMaskBuffer.isValid())
			{
				Con::errorf("Skyline Fatal Error: Unable to create maskbuffer");
				return false;
			}
			if (!mMaskTarget.registerWithName("skylineMask"))
			{
				Con::errorf("Skyline Fatal Error : Unable to register masktarget");
				return false;
			}
			mMaskTarget.setTexture(mMaskBuffer);
			GFXTextureManager::addEventDelegate(this, &SkyLine::_onTextureEvent);
		}
		else if (mMaskTarget.isRegistered())
		{
			GFXTextureManager::addEventDelegate(this, &SkyLine::_onTextureEvent);
		}

		if (mMaskShader == NULL)
		{
			ShaderData *shaderData;
			mMaskShader = Sim::findObject("SkyLineMskShader", shaderData) ?
				shaderData->getShader() : NULL;
			if (!mMaskShader)
			{
				Con::errorf("SkyLine::_updateMask() - could not find SkyLineShader");
				return false;
			}

			// Create ShaderConstBuffer and Handles

			mMaskShaderConsts = mMaskShader->allocConstBuffer();
			if (mMaskShaderConsts.isNull())
			{
				Con::errorf("SkyLine::_updateMask() - could not allocate ShaderConstants 2.");
				return false;
			}

			mMaskModelViewProjSC = mMaskShader->getShaderConstHandle("$modelView");
			mMaskObjTransSC = mMaskShader->getShaderConstHandle("$objTrans");
			mMaskEyePosSC = mMaskShader->getShaderConstHandle("$eyePosWorld");
			mAlphaSC = mMaskShader->getShaderConstHandle("$alphaMax");
		}

		if (!z_buf.isValid() || z_buf.isNull())
		{
			z_buf = GFX->allocRenderToTextureTarget();
			if (z_buf == NULL)
			{
				Con::errorf("SkyLine::_updateMask() - Could not create Render Target");
				return false;
			}
		}

		mMaskInit = true;

		return true;
	}
	else
	{
		Con::setBoolVariable("$LightRayPostFX::useMask", false);
		mMaskInit = false;
		if (mMaskTarget.isRegistered())
		{
			mMaskTarget.unregister();
			GFXTextureManager::removeEventDelegate(this, &SkyLine::_onTextureEvent);
		}
		if (mMaskBuffer.isValid())
		{
			mMaskBuffer->kill();
			mMaskBuffer = NULL;
		}
		return false;
	}
}

void SkyLine::EnableLRMask(bool OnOff)
{
	if (OnOff != mCreateMask)
	{
		mCreateMask = OnOff;
		setMaskBits(maskMask);
	}
	return;
}

DefineEngineMethod(SkyLine, Apply, void, (), , "")
{
	object->inspectPostApply();
}

DefineEngineMethod(SkyLine, SetLRMask, void, (bool OnOff), ,"Enables/Disables the Lightray PostFX mask")
{
	object->EnableLRMask(OnOff);
}