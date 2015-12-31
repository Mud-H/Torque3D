//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
//  This file contains shader data necessary for various engine utility functions
//-----------------------------------------------------------------------------


singleton ShaderData( ParticlesShaderData )
{
   DXVertexShaderFile     = "data/shaders/common/particlesV.hlsl";
   DXPixelShaderFile      = "data/shaders/common/particlesP.hlsl";   
   
   OGLVertexShaderFile     = "data/shaders/common/gl/particlesV.glsl";
   OGLPixelShaderFile      = "data/shaders/common/gl/particlesP.glsl";
   
   samplerNames[0] = "$diffuseMap";
   samplerNames[1] = "$prepassTex";
   samplerNames[2] = "$paraboloidLightMap";
   
   pixVersion = 2.0;
};

singleton ShaderData( OffscreenParticleCompositeShaderData )
{
   DXVertexShaderFile     = "data/shaders/common/particleCompositeV.hlsl";
   DXPixelShaderFile      = "data/shaders/common/particleCompositeP.hlsl";
   
   OGLVertexShaderFile     = "data/shaders/common/gl/particleCompositeV.glsl";
   OGLPixelShaderFile      = "data/shaders/common/gl/particleCompositeP.glsl";
   
   samplerNames[0] = "$colorSource";
   samplerNames[1] = "$edgeSource";
   
   pixVersion = 2.0;
};

//-----------------------------------------------------------------------------
// Planar Reflection
//-----------------------------------------------------------------------------
new ShaderData( ReflectBump )
{
   DXVertexShaderFile 	= "data/shaders/common/planarReflectBumpV.hlsl";
   DXPixelShaderFile 	= "data/shaders/common/planarReflectBumpP.hlsl";
   
   OGLVertexShaderFile  = "data/shaders/common/gl/planarReflectBumpV.glsl";
   OGLPixelShaderFile   = "data/shaders/common/gl/planarReflectBumpP.glsl";
              
   samplerNames[0] = "$diffuseMap";
   samplerNames[1] = "$refractMap";
   samplerNames[2] = "$bumpMap";
   
   pixVersion = 2.0;
};

new ShaderData( Reflect )
{
   DXVertexShaderFile 	= "data/shaders/common/planarReflectV.hlsl";
   DXPixelShaderFile 	= "data/shaders/common/planarReflectP.hlsl";
   
   OGLVertexShaderFile  = "data/shaders/common/gl/planarReflectV.glsl";
   OGLPixelShaderFile   = "data/shaders/common/gl/planarReflectP.glsl";
   
   samplerNames[0] = "$diffuseMap";
   samplerNames[1] = "$refractMap";
   
   pixVersion = 1.4;
};

//-----------------------------------------------------------------------------
// fxFoliageReplicator
//-----------------------------------------------------------------------------
new ShaderData( fxFoliageReplicatorShader )
{
   DXVertexShaderFile 	= "data/shaders/common/fxFoliageReplicatorV.hlsl";
   DXPixelShaderFile 	= "data/shaders/common/fxFoliageReplicatorP.hlsl";
   
   OGLVertexShaderFile  = "data/shaders/common/gl/fxFoliageReplicatorV.glsl";
   OGLPixelShaderFile   = "data/shaders/common/gl/fxFoliageReplicatorP.glsl";

   samplerNames[0] = "$diffuseMap";
   samplerNames[1] = "$alphaMap";
   
   pixVersion = 1.4;
};