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

#ifndef _SKYLINE_H_
#define _SKYLINE_H_

#ifndef _SCENEOBJECT_H_
#include "scene/sceneObject.h"
#endif
#ifndef _GFXDEVICE_H_
#include "gfx/gfxDevice.h"
#endif
#ifndef _CUBEMAPDATA_H_
#include "gfx/sim/cubemapData.h"
#endif
#ifndef _GFXSHADER_H_
#include "gfx/gfxshader.h"
#endif
#ifndef _GFXVERTEXBUFFER_H_
#include "gfx/gfxVertexBuffer.h"
#endif
#ifndef _GFXPRIMITIVEBUFFER_H_
#include "gfx/gfxPrimitiveBuffer.h"
#endif
#ifndef _MATTEXTURETARGET_H_
#include "materials/matTextureTarget.h"
#endif
#ifndef _SIGNAL_H_
#include "core/util/tSignal.h"
#endif

GFXDeclareVertexFormat(GFXSkyVertex)
{
	Point3F point;
	Point3F normal;
	Point2F texCoord;
	F32 up;
};

class MatrixSet;

class SkyLine : public SceneObject
{
	typedef SceneObject Parent;

	enum
	{
		skyLineMask = Parent::NextFreeMask,
		maskMask = Parent::NextFreeMask << 1,
		NextFreeMask = Parent::NextFreeMask << 2
	};
public:

	SkyLine();
	virtual ~SkyLine();

	DECLARE_CONOBJECT(SkyLine);

	virtual bool onAdd();
	virtual void onRemove();
	static void initPersistFields();
	virtual void inspectPostApply();

	virtual U32 packUpdate(NetConnection *conn, U32 mask, BitStream *stream);
	virtual void unpackUpdate(NetConnection *conn, BitStream *stream);

	void prepRenderImage(SceneRenderState* state);

	void _renderObject(ObjectRenderInst *ri, SceneRenderState *state, BaseMatInstance *mi);

	void _initMeshes();
	void EnableLRMask(bool OnOff);

protected:

	// Material 
	CubemapData *mSkyLineCubemap;
	String mSkyLineCubemapName;

	PlatformWindow* mPlatformWindow;

	GFXShaderRef mShader;
	GFXShaderConstBufferRef mShaderConsts;
	GFXShaderConstHandle *mModelViewProjSC;
	GFXShaderConstHandle *mObjTransSC;
	GFXShaderConstHandle *mEyePosSC;
	GFXShaderConstHandle *mAmbientColorSC;
	GFXShaderConstHandle *mHazeColorSC;
	GFXShaderConstHandle *mHazeDataSC;
	GFXShaderConstHandle *mHDRTypeSC;// ME Fix precomp shaders

	GFXStateBlockDesc mSBDesc;

	GFXShaderRef mMaskShader;
	GFXShaderConstBufferRef mMaskShaderConsts;
	GFXShaderConstHandle *mMaskModelViewProjSC;
	GFXShaderConstHandle *mMaskObjTransSC;
	GFXShaderConstHandle *mMaskEyePosSC;
	GFXShaderConstHandle *mAlphaSC;

	GFXVertexBufferHandle<GFXSkyVertex> mVB;
	U32 mPrimCount;

	GFXTextureTargetRef z_buf;
	GFXTexHandle mMaskBuffer;
	NamedTexTarget mMaskTarget;
	NamedTexTargetRef mMaskTargetRef;

	bool mDrawBottom;
	bool mIsVBDirty;
	bool mIsFogVBDirty;
	bool mCreateMask;
	bool mOldMask;
	bool mMaskInit;

	MatrixSet *mMatrixSet;

	F32 mFogBandEnd;

	void _onTextureEvent(GFXTexCallbackCode code);

	void _updateMaterial();
	bool _setupRender();
	bool _updateMask();
};

#endif // _SKYLINE_H_