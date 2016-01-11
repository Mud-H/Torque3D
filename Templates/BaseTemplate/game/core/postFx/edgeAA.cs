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


singleton GFXStateBlockData( PFX_DefaultEdgeAAStateBlock )
{
   zDefined = true;
   zEnable = false;
   zWriteEnable = false;
      
   samplersDefined = true;
   samplerStates[0] = SamplerClampPoint;
   //samplerStates[1] = SamplerWrapPoint;
};

singleton ShaderData( PFX_EdgeAADetectShader )
{   
   DXVertexShaderFile 	= "data/shaders/common/postFx/postFxV.hlsl";
   DXPixelShaderFile 	= "data/shaders/common/postFx/edgeaa/edgeDetectP.hlsl";
         
   OGLVertexShaderFile  = "data/shaders/common/postFx/gl/postFxV.glsl";
   OGLPixelShaderFile   = "data/shaders/common/postFx/edgeaa/gl/edgeDetectP.glsl";
      
   samplerNames[0] = "$prepassBuffer";
   
   pixVersion = 3.0;
};

singleton ShaderData( PFX_EdgeAAShader )
{   
   DXVertexShaderFile 	= "data/shaders/common/postFx/edgeaa/edgeAAV.hlsl";
   DXPixelShaderFile 	= "data/shaders/common/postFx/edgeaa/edgeAAP.hlsl";
         
   OGLVertexShaderFile  = "data/shaders/common/postFx/edgeaa/gl/edgeAAV.glsl";
   OGLPixelShaderFile   = "data/shaders/common/postFx/edgeaa/gl/edgeAAP.glsl";
      
   samplerNames[0] = "$edgeBuffer";
   samplerNames[1] = "$backBuffer";
   
   pixVersion = 3.0;
};

singleton ShaderData( PFX_EdgeAADebugShader )
{   
   DXVertexShaderFile 	= "data/shaders/common/postFx/postFxV.hlsl";
   DXPixelShaderFile 	= "data/shaders/common/postFx/edgeaa/dbgEdgeDisplayP.hlsl";
         
   OGLVertexShaderFile  = "data/shaders/common/postFx/gl/postFxV.glsl";
   OGLPixelShaderFile   = "data/shaders/common/postFx/edgeaa/gl/dbgEdgeDisplayP.glsl";
      
   samplerNames[0] = "$edgeBuffer";
   
   pixVersion = 3.0;
};

singleton PostEffect( EdgeDetectPostEffect )
{
   renderTime = "PFXBeforeBin";
   renderBin = "ObjTranslucentBin";      
   //renderPriority = 0.1;
   targetScale = "0.5 0.5";
      
   shader = PFX_EdgeAADetectShader;
   stateBlock = PFX_DefaultEdgeAAStateBlock;
   texture[0] = "#prepass";
   target = "#edge";
   
   isEnabled = true;
};

singleton PostEffect( EdgeAAPostEffect )
{
   renderTime = "PFXAfterDiffuse";
   //renderBin = "ObjTranslucentBin";      
   //renderPriority = 0.1;
   
   shader = PFX_EdgeAAShader;
   stateBlock = PFX_DefaultEdgeAAStateBlock;
   texture[0] = "#edge"; 
   texture[1] = "$backBuffer";
   target = "$backBuffer";
};
   
singleton PostEffect( Debug_EdgeAAPostEffect )
{
   renderTime = "PFXAfterDiffuse";
   //renderBin = "ObjTranslucentBin";      
   //renderPriority = 0.1;
      
   shader = PFX_EdgeAADebugShader;
   stateBlock = PFX_DefaultEdgeAAStateBlock;
   texture[0] = "#edge"; 
   target = "$backBuffer";
};