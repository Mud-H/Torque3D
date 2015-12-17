//options settings

//Screen and Display menu
//Renderer Mode
//Screen resolution
//Windowed/fullscreen(borderless?)
//VSync

//Screen brightness
//screen brightness
//screen gamma

//Lighting Menu
//Shadow Distance(Distance shadows are drawn to. Also affects shadowmap slices)
//Shadow Quality(Resolution of shadows rendered, setting to none disables dynamic shadows)
//Soft Shadows(Whether shadow softening is used)
//Shadow caching(If the lights enable it, shadow caching is activated)
//Light Draw Distance(How far away lights are still drawn. Doesn't impact vector lights like the sun)

//Mesh and Textures Menu
//Draw distance(Overall draw distance) -slider
//Object draw distance(Draw distance from small/unimportant objects) -slider
//Mesh quality
//Texture quality
//Foliage draw distance
//Terrain Quality
//Decal Quality

//Effects Menu
//Parallax
//HDR
//Light shafts
//Motion Blur
//Depth of Field
//SSAO
//AA(ModelXAmount)[defualt is FXAA]
//Anisotropic filtering

//Keybinds

//Camera
//horizontal mouse sensitivity
//vert mouse sensitivity
//invert vertical
//zoom mouse sensitivities(both horz/vert)
//headbob
//FOV

function OptionsMenu::onWake(%this)
{
    OptionsMain.hidden = false;
    ControlsMenu.hidden = true;
    GraphicsMenu.hidden = true;
    CameraMenu.hidden = true;
    ScreenBrightnessMenu.hidden = true;
    
    OptionsOKButton.hidden = false;
    OptionsCancelButton.hidden = false;
    OptionsDefaultsButton.hidden = false;

    //if(!isObject(%this.tamlReader))
        %this.tamlReader = new Taml();

    echo("OPTIONS MENU TAML READER: " @ %this.tamlReader);
}

function OptionsMenuOKButton::onClick(%this)
{
    //save the settings and then back out
    
    OptionsMenu.backOut();
}

function OptionsMenuCancelButton::onClick(%this)
{
    //we don't save, so go straight to backing out of the menu    
    OptionsMenu.backOut();
}

function OptionsMenuDefaultsButton::onClick(%this)
{
    //we don't save, so go straight to backing out of the menu    
    OptionsMenu.backOut();
}

function ControlsSettingsMenuButton::onClick(%this)
{
    OptionsMain.hidden = true;
    ControlsMenu.hidden = false;
}

function GraphicsSettingsMenuButton::onClick(%this)
{
    OptionsMain.hidden = true;
    GraphicsMenu.hidden = false;

    GraphicsMenu.loadSettings();
}

function CameraSettingsMenuButton::onClick(%this)
{
    OptionsMain.hidden = true;
    CameraMenu.hidden = false;
    
    CameraMenu.loadSettings();
}

function AudioSettingsMenuButton::onClick(%this)
{
    OptionsMain.hidden = true;
    AudioMenu.hidden = false;
    AudioMenu.loadSettings();
}

function ScreenBrSettingsMenuButton::onClick(%this)
{
    OptionsMain.hidden = true;
    ScreenBrightnessMenu.hidden = false;
}

function OptionsMenu::backOut(%this)
{
   //save the settings and then back out
   if(OptionsMain.hidden == false)
   {
      //we're not in a specific menu, so we're actually exiting
      Canvas.popDialog(OptionsMenu);

      if(isObject(OptionsMenu.returnGui) && OptionsMenu.returnGui.isMethod("onReturnTo"))
         OptionsMenu.returnGui.onReturnTo();
   }
   else
   {
      OptionsMain.hidden = false;
      ControlsMenu.hidden = true;
      GraphicsMenu.hidden = true;
      CameraMenu.hidden = true;
      AudioMenu.hidden = true;
      ScreenBrightnessMenu.hidden = true;
   }
}

function OptionsMenu::addSettingOption(%this, %arrayTarget)
{
    %graphicsOption = OptionsMenu.tamlReader.read("gui/GraphicsMenuSettingsCtrl.taml");

    %arrayTarget.add(%graphicsOption);

    return %graphicsOption;
}

function OptionsMenu::addSliderOption(%this, %arrayTarget, %range, %ticks, %variable, %value, %class)
{
    %graphicsOption = OptionsMenu.tamlReader.read("gui/GraphicsMenuSettingsSlider.taml");

    %arrayTarget.add(%graphicsOption);
    
    if(%range !$= "")
    {
       %graphicsOption-->slider.range = %range;
    }
    
    if(%ticks !$= "")
    {
       %graphicsOption-->slider.ticks = %ticks;
    }
    
    if(%variable !$= "")
    {
       %graphicsOption-->slider.variable = %variable;
    }
    
    if(%value !$= "")
    {
       %graphicsOption-->slider.setValue(%value);
    }
    
    if(%class !$= "")
    {
       %graphicsOption-->slider.className = %class;
    }
    else
        %graphicsOption-->slider.className = OptionsMenuSlider;
        
    %graphicsOption-->slider.snap = true;
    
    %graphicsOption-->slider.onValueSet();

    return %graphicsOption;
}

function OptionsMenuSlider::onMouseDragged(%this)
{
   %this.onValueSet();
}

function OptionsMenuSlider::onValueSet(%this)
{
   %this.getParent().getParent()-->valueText.setText(mRound(%this.value * 10));  
}

function FOVOptionSlider::onMouseDragged(%this)
{
   %this.onValueSet();
}

function FOVOptionSlider::onValueSet(%this)
{
   %this.getParent().getParent()-->valueText.setText(mRound(%this.value));
}

/// Returns true if the current quality settings equal
/// this graphics quality level.
function OptionsMenuSettingLevel::isCurrent( %this )
{
   // Test each pref to see if the current value
   // equals our stored value.
   
   for ( %i=0; %i < %this.count(); %i++ )
   {
      %pref = %this.getKey( %i );
      %value = %this.getValue( %i );
      
      %prefVarValue = getVariable( %pref );
      if ( getVariable( %pref ) !$= %value )
         return false;
   }
   
   return true;
}
// =============================================================================
// CAMERA MENU
// =============================================================================
function CameraMenu::onWake(%this)
{
    
}

function CameraMenu::apply(%this)
{
   setFOV($pref::Player::defaultFov);  
}

function CameraMenu::loadSettings(%this)
{
   CameraMenuOptionsArray.clear();
   
   %option = OptionsMenu.addSettingOption(CameraMenuOptionsArray);
   %option-->nameText.setText("Invert Vertical");
   %option.qualitySettingGroup = InvertVerticalMouse;
   %option.init();
   
   %option = OptionsMenu.addSliderOption(CameraMenuOptionsArray, "0.1 1", 8, "$pref::Input::VertMouseSensitivity", $pref::Input::VertMouseSensitivity);
   %option-->nameText.setText("Vertical Sensitivity");
   
   %option = OptionsMenu.addSliderOption(CameraMenuOptionsArray, "0.1 1", 8, "$pref::Input::HorzMouseSensitivity", $pref::Input::HorzMouseSensitivity);
   %option-->nameText.setText("Horizontal Sensitivity");
   
   %option = OptionsMenu.addSliderOption(CameraMenuOptionsArray, "0.1 1", 8, "$pref::Input::ZoomVertMouseSensitivity", $pref::Input::ZoomVertMouseSensitivity);
   %option-->nameText.setText("Zoom Vertical Sensitivity");

   %option = OptionsMenu.addSliderOption(CameraMenuOptionsArray, "0.1 1", 8, "$pref::Input::ZoomHorzMouseSensitivity", $pref::Input::ZoomHorzMouseSensitivity);
   %option-->nameText.setText("Zoom Horizontal Sensitivity");
   
   %option = OptionsMenu.addSliderOption(CameraMenuOptionsArray, "65 90", 25, "$pref::Player::defaultFov", $pref::Player::defaultFov, FOVOptionSlider);
   %option-->nameText.setText("Field of View");
   
   CameraMenuOptionsArray.refresh();
}

function CameraMenuOKButton::onClick(%this)
{
   //save the settings and then back out
    CameraMenu.apply();
    OptionsMenu.backOut();
}

// =============================================================================
// GRAPHICS MENU
// =============================================================================
//Mesh and Textures
//
function GraphicsMenu::onWake(%this)
{
    %this.settingsPage = 0;
}

function GraphicsMenuOKButton::onClick(%this)
{
    //save the settings and then back out
    GraphicsMenu.apply();
    OptionsMenu.backOut();
}

function GraphicsMenu::loadSettings(%this)
{
   %this.loadDisplayOptions();
   %this.loadLightingOptions();
   %this.loadMeshesAndTexturesOptions();
   %this.loadShaderOptions();
   %this.changeSettingsPage();
}

function GraphicsMenu::changeSettingsPage(%this)
{
    %lastPageIndex = 3;

    if(%this.settingsPage < 0)
        %this.settingsPage = 0;
        
    else if(%this.settingsPage > %lastPageIndex)
        %this.settingsPage = %lastPageIndex;

    if(%this.settingsPage == 0)
    {
        GraphisMenuPageText.setText("Display");

        ScreenSettingsMenu.hidden = false;
        GraphicsLightingSettingsMenu.hidden = true;
        GraphicsShaderSettingsMenu.hidden = true;
        GraphicsMeshTexSettingsMenu.hidden = true;
    }
    if(%this.settingsPage == 1)
    {
        GraphisMenuPageText.setText("Lighting");

        ScreenSettingsMenu.hidden = true;
        GraphicsLightingSettingsMenu.hidden = false;
        GraphicsShaderSettingsMenu.hidden = true;
        GraphicsMeshTexSettingsMenu.hidden = true;
    }
    else if(%this.settingsPage == 2)
    {
        GraphisMenuPageText.setText("Meshes and Textures");
        
        ScreenSettingsMenu.hidden = true;
        GraphicsLightingSettingsMenu.hidden = true;
        GraphicsShaderSettingsMenu.hidden = true;
        GraphicsMeshTexSettingsMenu.hidden = false;
    }
    else if(%this.settingsPage == 3)
    {
        GraphisMenuPageText.setText("Shaders");
        
        ScreenSettingsMenu.hidden = true;
        GraphicsLightingSettingsMenu.hidden = true;
        GraphicsShaderSettingsMenu.hidden = false;
        GraphicsMeshTexSettingsMenu.hidden = true;
    }
}

//
function GraphicsMenu::Autodetect(%this)
{
   $pref::Video::autoDetect = false;
   
   %shaderVer = getPixelShaderVersion();
   %intel = ( strstr( strupr( getDisplayDeviceInformation() ), "INTEL" ) != -1 ) ? true : false;
   %videoMem = GFXCardProfilerAPI::getVideoMemoryMB();
   
   return %this.Autodetect_Apply( %shaderVer, %intel, %videoMem );
}

function GraphicsMenu::Autodetect_Apply(%this, %shaderVer, %intel, %videoMem )
{
   if ( %shaderVer < 2.0 )
   {      
      return "Your video card does not meet the minimum requirment of shader model 2.0.";
   }
   
   if ( %shaderVer < 3.0 || %intel )
   {
      // Allow specular and normals for 2.0a and 2.0b
      if ( %shaderVer > 2.0 )
      {
         MeshQualityGroup-->Lowest.apply();
         TextureQualityGroup-->Lowest.apply();
         GroundCoverDensityGroup-->Lowest.apply();
         DecalLifetimeGroup-->None.apply();
         TerrainQualityGroup-->Lowest.apply();
         
         ShadowQualityList-->None.apply();
         ShadowDistanceList-->Lowest.apply();   
         SoftShadowList-->Off.apply();
         ShadowCacheList-->On.apply();
         LightDistanceList-->Lowest.apply();
         
         ShaderQualityGroup-->Normal.apply();
         ReflectionsQualityGroup-->Off.apply();
         ParallaxQualityGroup-->Off.apply();
         HDRQualityGroup-->Off.apply();
         SSAOQualityGroup-->Off.apply();
         LightRaysQualityGroup-->Off.apply();
         DOFQualityGroup-->Off.apply();
         VignetteQualityGroup-->Off.apply();   
      }
      else
      {
         MeshQualityGroup-->Lowest.apply();
         TextureQualityGroup-->Lowest.apply();
         GroundCoverDensityGroup-->Lowest.apply();
         DecalLifetimeGroup-->None.apply();
         TerrainQualityGroup-->Lowest.apply();
         
         ShadowQualityList-->None.apply();
         ShadowDistanceList-->Lowest.apply();   
         SoftShadowList-->Off.apply();
         ShadowCacheList-->On.apply();
         LightDistanceList-->Lowest.apply();
         
         ShaderQualityGroup-->Low.apply();
         ReflectionsQualityGroup-->Off.apply();
         ParallaxQualityGroup-->Off.apply();
         HDRQualityGroup-->Off.apply();
         SSAOQualityGroup-->Off.apply();
         LightRaysQualityGroup-->Off.apply();
         DOFQualityGroup-->Off.apply();
         VignetteQualityGroup-->Off.apply();
      }
   }   
   else
   {
      if ( %videoMem > 1000 )
      {
         MeshQualityGroup-->High.apply();
         TextureQualityGroup-->High.apply();
         GroundCoverDensityGroup-->High.apply();
         DecalLifetimeGroup-->High.apply();
         TerrainQualityGroup-->High.apply();
         
         ShadowQualityList-->High.apply();
         ShadowDistanceList-->Highest.apply();   
         SoftShadowList-->High.apply();
         ShadowCacheList-->On.apply();
         LightDistanceList-->Highest.apply();
         
         ShaderQualityGroup-->Normal.apply();
         ReflectionsQualityGroup-->On.apply();
         ParallaxQualityGroup-->On.apply();
         HDRQualityGroup-->On.apply();
         SSAOQualityGroup-->On.apply();
         LightRaysQualityGroup-->On.apply();
         DOFQualityGroup-->On.apply();
         VignetteQualityGroup-->On.apply();
      }
      else if ( %videoMem > 400 || %videoMem == 0 )
      {
         MeshQualityGroup-->Medium.apply();
         TextureQualityGroup-->Medium.apply();
         GroundCoverDensityGroup-->Medium.apply();
         DecalLifetimeGroup-->Medium.apply();
         TerrainQualityGroup-->Medium.apply();
         
         ShadowQualityList-->Medium.apply();
         ShadowDistanceList-->Medium.apply();   
         SoftShadowList-->Low.apply();
         ShadowCacheList-->On.apply();
         LightDistanceList-->Medium.apply();
         
         ShaderQualityGroup-->Normal.apply();
         ReflectionsQualityGroup-->On.apply();
         ParallaxQualityGroup-->Off.apply();
         HDRQualityGroup-->On.apply();
         SSAOQualityGroup-->Off.apply();
         LightRaysQualityGroup-->On.apply();
         DOFQualityGroup-->On.apply();
         VignetteQualityGroup-->On.apply();
         
         if ( %videoMem == 0 )
            return "Torque was unable to detect available video memory. Applying 'Medium' quality.";
      }
      else
      {
         MeshQualityGroup-->Low.apply();
         TextureQualityGroup-->Low.apply();
         GroundCoverDensityGroup-->Low.apply();
         DecalLifetimeGroup-->Low.apply();
         TerrainQualityGroup-->Low.apply();
         
         ShadowQualityList-->Low.apply();
         ShadowDistanceList-->Low.apply();   
         SoftShadowList-->Low.apply();
         ShadowCacheList-->On.apply();
         LightDistanceList-->Low.apply();
         
         ShaderQualityGroup-->Normal.apply();
         ReflectionsQualityGroup-->On.apply();
         ParallaxQualityGroup-->Off.apply();
         HDRQualityGroup-->Off.apply();
         SSAOQualityGroup-->Off.apply();
         LightRaysQualityGroup-->Off.apply();
         DOFQualityGroup-->Off.apply();
         VignetteQualityGroup-->Off.apply();
      }
   }
   
   echo("Exporting client prefs");
   export("$pref::*", "scripts/client/prefs.cs", false);
   
   return "Graphics quality settings have been auto detected.";
}

//
function GraphicsMenu::loadDrawDistanceOptions(%this)
{
   %option = OptionsMenu.addSliderOption();
   %option-->nameText.setText("View Distance");
   %option-->slider.variable = "$pref::Video::Gamma";
}

function GraphicsMenu::loadShaderOptions(%this)
{
   GraphicsShaderOptionsArray.clear();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("Material Quality");
   %option.qualitySettingGroup = ShaderQualityGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("Parallax");
   %option.qualitySettingGroup = ParallaxQualityGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("Water Reflections");
   %option.qualitySettingGroup = ReflectionsQualityGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("High Dynamic Range");
   %option.qualitySettingGroup = HDRQualityGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("Sceen Space Ambient Occlusion");
   %option.qualitySettingGroup = SSAOQualityGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("Light Rays");
   %option.qualitySettingGroup = LightRaysQualityGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("Depth of Field");
   %option.qualitySettingGroup = DOFQualityGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(GraphicsShaderOptionsArray);
   %option-->nameText.setText("Vignetting");
   %option.qualitySettingGroup = VignetteQualityGroup;
   %option.init();
   
   GraphicsShaderOptionsArray.refresh();
}

function GraphicsMenu::loadLightingOptions(%this)
{
   GraphicsLightingOptionsArray.clear();
   
    %option = OptionsMenu.addSettingOption(GraphicsLightingOptionsArray);
    %option-->nameText.setText("Shadow Quality");
    %option.qualitySettingGroup = ShadowQualityList;
    %option.init();

    %option = OptionsMenu.addSettingOption(GraphicsLightingOptionsArray);
    %option-->nameText.setText("Shadow Caching");
    %option.qualitySettingGroup = ShadowCacheList;
    %option.init();

    %option = OptionsMenu.addSettingOption(GraphicsLightingOptionsArray);
    %option-->nameText.setText("Soft Shadows");
    %option.qualitySettingGroup = SoftShadowList;
    %option.init();
    
    GraphicsLightingOptionsArray.refresh();
}

function GraphicsMenu::loadMeshesAndTexturesOptions(%this)
{
   GraphicsMeshTexOptionsArray.clear();
   
    %option = OptionsMenu.addSettingOption(GraphicsMeshTexOptionsArray);
    %option-->nameText.setText("Mesh Detail");
    %option.qualitySettingGroup = MeshQualityGroup;
    %option.init();

    %option = OptionsMenu.addSettingOption(GraphicsMeshTexOptionsArray);
    %option-->nameText.setText("Texture Detail");
    %option.qualitySettingGroup = TextureQualityGroup;
    %option.init();
    
    %option = OptionsMenu.addSettingOption(GraphicsMeshTexOptionsArray);
    %option-->nameText.setText("Ground Clutter Density");
    %option.qualitySettingGroup = GroundCoverDensityGroup;
    %option.init();
    
    %option = OptionsMenu.addSettingOption(GraphicsMeshTexOptionsArray);
    %option-->nameText.setText("Terrain Detail");
    %option.qualitySettingGroup = TerrainQualityGroup;
    %option.init();
    
    %option = OptionsMenu.addSettingOption(GraphicsMeshTexOptionsArray);
    %option-->nameText.setText("Decal Lifetime");
    %option.qualitySettingGroup = DecalLifetimeGroup;
    %option.init();
    
    //%option = OptionsMenu.addSliderOption();
    //%option-->nameText.setText("View Distance");
    //%option-->SettingText.setText("On");
    //%option.qualitySettingGroup = ViewDistanceQualityGroup;
    
    GraphicsMeshTexOptionsArray.refresh();
}

function GraphicsMenu::loadDisplayOptions(%this)
{
   ScreenOptionsArray.clear();
   
   %option = OptionsMenu.addSettingOption(ScreenOptionsArray);
   %option-->nameText.setText("Graphics Driver");
   %option.qualitySettingGroup = DisplayAPISettingGroup;
   %option.init();

   //Resolution
   %option = OptionsMenu.addSettingOption(ScreenOptionsArray);
   %option-->nameText.setText("Screen Resolution");
    
   // Loop through all and add all valid resolutions
   if(!isObject(ScreenResolutionSettingGroup))
   {
      new SimGroup( ScreenResolutionSettingGroup );
   }
   else
   {
      ScreenResolutionSettingGroup.clear();
   }
   
   %count = 0;
   %resCount = Canvas.getModeCount();
   for (%i = 0; %i < %resCount; %i++)
   {
      %testResString = Canvas.getMode( %i );
      %testRes = _makePrettyResString( %testResString );
                     
      // Only add to list if it isn't there already.
      //if (%resMenu.findText(%testRes) == -1)
      //{
         //%resMenu.add(%testRes, %i);
         
         %setting = new ArrayObject()
         {
            class = "OptionsMenuSettingLevel";
            caseSensitive = true;
            
            displayName = %testRes;
            
            key["$pref::Video::Resolution"] = getWords(%testResString, 0, 1);
         };
         
         ScreenResolutionSettingGroup.add(%setting);
   
         %count++;
      //}
   }
   %option.qualitySettingGroup = ScreenResolutionSettingGroup;
   %option.init();
   //

    %option = OptionsMenu.addSettingOption(ScreenOptionsArray);
    %option-->nameText.setText("Full Screen");
    %option.qualitySettingGroup = FullScreenSettingGroup;
    %option.init();
    
    %option = OptionsMenu.addSettingOption(ScreenOptionsArray);
    %option-->nameText.setText("Refresh Rate");
    %option.qualitySettingGroup = RefreshRateQualityGroup;
    %option.init();
    
    %option = OptionsMenu.addSettingOption(ScreenOptionsArray);
    %option-->nameText.setText("Anisotropic Filtering");
    %option.qualitySettingGroup = AnisoQualityGroup;
    %option.init();
    
    %option = OptionsMenu.addSettingOption(ScreenOptionsArray);
    %option-->nameText.setText("AntiAliasing");
    %option.qualitySettingGroup = AAQualityGroup;
    %option.init();
    
    %option = OptionsMenu.addSettingOption(ScreenOptionsArray);
    %option-->nameText.setText("VSync");
    %option.qualitySettingGroup = VSyncQualityGroup;
    %option.init();
    
    ScreenOptionsArray.refresh();
}

function _makePrettyResString( %resString )
{
   %width = getWord( %resString, $WORD::RES_X );
   %height = getWord( %resString, $WORD::RES_Y );
   
   %aspect = %width / %height;
   %aspect = mRound( %aspect * 100 ) * 0.01;            
   
   switch$( %aspect )
   {
      case "1.33":
         %aspect = "4:3";
      case "1.78":
         %aspect = "16:9";
      default:
         %aspect = "";
   }
   
   %outRes = %width @ " x " @ %height;
   if ( %aspect !$= "" )
      %outRes = %outRes @ "  (" @ %aspect @ ")";
      
   return %outRes;   
}

//
function GraphicsMenuSetting::init( %this )
{
   assert( isObject( %this ) );
   assert( isObject( %this.qualitySettingGroup ) );
    
   // Fill it.
   %select = -1;
   %selectedName = "";
   for ( %i=0; %i < %this.qualitySettingGroup.getCount(); %i++ )
   {
      %level = %this.qualitySettingGroup.getObject( %i );
      
      %levelName = %level.displayName;
      if ( %level.isCurrent() )
      {
         %select = %i;
         %selectedName = %level.displayName;
      }
   }
   
   // Setup a default selection.
   if ( %select == -1 )
   {
      %this-->SettingText.setText( "Custom" );
      %this.selectedLevel = %this.qualitySettingGroup.getCount();
   }
   else
   {
      %this-->SettingText.setText(%selectedName);
      %this.selectedLevel = %select;
   }
}

function GraphicsMenu::apply(%this)
{
   //go through our settings and apply the changes!  
   for(%i=0; %i < GraphicsLightingOptionsArray.getCount(); %i++)
   {
      %setting = GraphicsLightingOptionsArray.getObject(%i);
      
      if(%setting.qualitySettingGroup.isMethod("onApply"))
         %setting.qualitySettingGroup.onApply();
   }
   
   for(%i=0; %i < GraphicsShaderOptionsArray.getCount(); %i++)
   {
      %setting = GraphicsShaderOptionsArray.getObject(%i);
      
      if(%setting.qualitySettingGroup.isMethod("onApply"))
         %setting.qualitySettingGroup.onApply();
   }
   
   for(%i=0; %i < GraphicsMeshTexOptionsArray.getCount(); %i++)
   {
      %setting = GraphicsMeshTexOptionsArray.getObject(%i);
      
      if(%setting.qualitySettingGroup.isMethod("onApply"))
         %setting.qualitySettingGroup.onApply();
   }
   
   //Update the display settings now
   %newBpp        = 32; // ... its not 1997 anymore.

   if ( $pref::Video::FullScreen == false )
	{
      // If we're in windowed mode switch the fullscreen check
      // if the resolution is bigger than the desktop.
      %deskRes    = getDesktopResolution();      
      %deskResX   = getWord(%deskRes, $WORD::RES_X);
      %deskResY   = getWord(%deskRes, $WORD::RES_Y);
	   if (  getWord( %newRes, $WORD::RES_X ) > %deskResX || 
	         getWord( %newRes, $WORD::RES_Y ) > %deskResY )
      {
         $pref::Video::FullScreen = true;
      }
	}

   // Build the final mode string.
	%newMode = $pref::Video::Resolution SPC $pref::Video::FullScreen SPC %newBpp SPC $pref::Video::RefreshRate SPC $pref::Video::AA;
	
   // Change the video mode.   
   if (  %newMode !$= $pref::Video::mode )
   {
      //if ( %testNeedApply )
      //   return true;

      $pref::Video::mode = %newMode;
      configureCanvas();
   }
   
   echo("Exporting client prefs");
   export("$pref::*", "scripts/client/prefs.cs", false);

 
}
function TextureQualityGroup::onApply( %this )
{
   // Note that this can be a slow operation.  
   reloadTextures();
}
function LightingQualityGroup::onApply( %this, %level )
{
   // Set the light manager.  This should do nothing 
   // if its already set or if its not compatible.   
   setLightManager( $pref::lightManager );
}
function DOFQualityGroup::onApply(%this)
{
   PostFXManager.settingsEffectSetEnabled("DOF", $pref::PostFX::EnableDOF);
}
function SSAOQualityGroup::onApply(%this)
{
   PostFXManager.settingsEffectSetEnabled("SSAO", $pref::PostFX::EnableSSAO);
}
function HDRQualityGroup::onApply(%this)
{
   PostFXManager.settingsEffectSetEnabled("HDR", $pref::PostFX::EnableHDR);
}
function LightRaysQualityGroup::onApply(%this)
{
   PostFXManager.settingsEffectSetEnabled("LightRays", $pref::PostFX::EnableLightRays);
}
function VignetteQualityGroup::onApply(%this)
{
   PostFXManager.settingsEffectSetEnabled("Vignette", $pref::PostFX::EnableVignette);
}

// =============================================================================
// AUDIO MENU
// =============================================================================
$AudioTestHandle = 0;
// Description to use for playing the volume test sound.  This isn't
// played with the description of the channel that has its volume changed
// because we know nothing about the playback state of the channel.  If it
// is paused or stopped, the test sound would not play then.
$AudioTestDescription = new SFXDescription()
{
   sourceGroup = AudioChannelMaster;
};

function AudioMenu::loadSettings(%this)
{
   // Loop through all and add all valid resolutions
   if(!isObject(SoundAPIGroup))
   {
      new SimGroup( SoundAPIGroup );
   }
   else
   {
      SoundAPIGroup.clear();
   }
   
   if(!isObject(SoundDeviceGroup))
   {
      new SimGroup( SoundDeviceGroup );
   }
   else
   {
      SoundDeviceGroup.clear();
   }
   
   %buffer = sfxGetAvailableDevices();
   %count = getRecordCount( %buffer );
   for (%i = 0; %i < %count; %i++)
   {
      %record = getRecord(%buffer, %i);
      %provider = getField(%record, 0);
      %device = getField(%record, 1);
         
      %setting = new ArrayObject()
      {
         class = "OptionsMenuSettingLevel";
         caseSensitive = true;
         
         displayName = %provider;
         
         key["$pref::SFX::API"] = %provider;
      };
      
      SoundAPIGroup.add(%setting);
      
      %setting = new ArrayObject()
      {
         class = "OptionsMenuSettingLevel";
         caseSensitive = true;
         
         displayName = %device;
         
         key["$pref::SFX::Device"] = %device;
      };
      
      SoundDeviceGroup.add(%setting);
   }
   
   AudioMenuOptionsArray.clear();
   
   %option = OptionsMenu.addSettingOption(AudioMenuOptionsArray);
   %option-->nameText.setText("Sound Provider");
   %option.qualitySettingGroup = SoundAPIGroup;
   %option.init();
   
   %option = OptionsMenu.addSettingOption(AudioMenuOptionsArray);
   %option-->nameText.setText("Sound Device");
   %option.qualitySettingGroup = SoundDeviceGroup;
   %option.init();
   
   %option = OptionsMenu.addSliderOption(AudioMenuOptionsArray, "0.1 1", 8,
      "$pref::SFX::masterVolume", $pref::SFX::masterVolume);
   %option-->nameText.setText("Master Audio Volume");
   
   %option = OptionsMenu.addSliderOption(AudioMenuOptionsArray, "0.1 1", 8,
      "$pref::SFX::channelVolume[AudioGui]", $pref::SFX::channelVolume[AudioGui]);
   %option-->nameText.setText("Gui Volume");
   
   %option = OptionsMenu.addSliderOption(AudioMenuOptionsArray, "0.1 1", 8, 
      "$pref::SFX::channelVolume[AudioEffect]", $pref::SFX::channelVolume[AudioEffect]);
   %option-->nameText.setText("Effect Volume");
   
   %option = OptionsMenu.addSliderOption(AudioMenuOptionsArray, "0.1 1", 8, 
      "$pref::SFX::channelVolume[AudioMusic]", $pref::SFX::channelVolume[AudioMusic]);
   %option-->nameText.setText("Music Volume");
   
   AudioMenuOptionsArray.refresh();
}

function AudioMenu::apply(%this)
{
   sfxSetMasterVolume( $pref::SFX::masterVolume );
   
   sfxSetChannelVolume( AudioGui, $pref::SFX::channelVolume[ AudioGui ] );
   sfxSetChannelVolume( AudioEffect, $pref::SFX::channelVolume[ AudioEffect ] );
   sfxSetChannelVolume( AudioMusic, $pref::SFX::channelVolume[ AudioMusic ] );

   if( !isObject( $AudioTestHandle ) )
   {
      $AudioTestDescription.volume = %volume;
      $AudioTestHandle = sfxPlayOnce( $AudioTestDescription, "art/sound/ui/volumeTest.wav" );
   }
}

function AudioMenuOKButton::onClick(%this)
{
   //save the settings and then back out
    AudioMenu.apply();
    OptionsMenu.backOut();
}

//
function OptionsMenuBackSetting::onClick(%this)
{
   %optionSetting = %this.getParent().getParent();
   %qualityGroup = %optionSetting.qualitySettingGroup;
   
   //now, lower our setting
   if(%optionSetting.selectedLevel > 0)
   {
      %newLevel = %optionSetting.selectedLevel-1;
      %newQualityGroup = %optionSetting.qualitySettingGroup.getObject( %newLevel );
      
      for ( %i=0; %i < %newQualityGroup.count(); %i++ )
      {
         %pref = %newQualityGroup.getKey( %i );
         %value = %newQualityGroup.getValue( %i );
         setVariable( %pref, %value );
      }
      
      %optionSetting-->SettingText.setText(%newQualityGroup.displayName);
      %optionSetting.selectedLevel = %newLevel;
   }
}

function OptionsMenuForwardSetting::onClick(%this)
{
   %optionSetting = %this.getParent().getParent();
   %qualityGroup = %optionSetting.qualitySettingGroup;
   
   //now, lower our setting
   %maxLevel = %qualityGroup.getCount()-1;
   if(%optionSetting.selectedLevel < %qualityGroup.getCount()-1)
   {
      %newLevel = %optionSetting.selectedLevel+1;
      %newQualityGroup = %optionSetting.qualitySettingGroup.getObject( %newLevel );
      
      for ( %i=0; %i < %newQualityGroup.count(); %i++ )
      {
         %pref = %newQualityGroup.getKey( %i );
         %value = %newQualityGroup.getValue( %i );
         setVariable( %pref, %value );
      }
      
      %optionSetting-->SettingText.setText(%newQualityGroup.displayName);
      %optionSetting.selectedLevel = %newLevel;
   }
}

// =============================================================================
// KEYBINDS MENU
// =============================================================================
$RemapCount = 0;
$RemapName[$RemapCount] = "Forward";
$RemapCmd[$RemapCount] = "moveforward";
$RemapGroup[$RemapCount] = "Movement";
$RemapCount++;
$RemapName[$RemapCount] = "Backward";
$RemapCmd[$RemapCount] = "movebackward";
$RemapGroup[$RemapCount] = "Movement";
$RemapCount++;
$RemapName[$RemapCount] = "Strafe Left";
$RemapCmd[$RemapCount] = "moveleft";
$RemapGroup[$RemapCount] = "Movement";
$RemapCount++;
$RemapName[$RemapCount] = "Strafe Right";
$RemapCmd[$RemapCount] = "moveright";
$RemapGroup[$RemapCount] = "Movement";
$RemapCount++;
$RemapName[$RemapCount] = "Jump";
$RemapCmd[$RemapCount] = "jump";
$RemapGroup[$RemapCount] = "Movement";
$RemapCount++;

$RemapName[$RemapCount] = "Fire Weapon";
$RemapCmd[$RemapCount] = "mouseFire";
$RemapGroup[$RemapCount] = "Combat";
$RemapCount++;
$RemapName[$RemapCount] = "Adjust Zoom";
$RemapCmd[$RemapCount] = "setZoomFov";
$RemapGroup[$RemapCount] = "Combat";
$RemapCount++;
$RemapName[$RemapCount] = "Toggle Zoom";
$RemapCmd[$RemapCount] = "toggleZoom";
$RemapGroup[$RemapCount] = "Combat";
$RemapCount++;

$RemapName[$RemapCount] = "Free Look";
$RemapCmd[$RemapCount] = "toggleFreeLook";
$RemapGroup[$RemapCount] = "Miscellaneous";
$RemapCount++;
$RemapName[$RemapCount] = "Switch 1st/3rd";
$RemapCmd[$RemapCount] = "toggleFirstPerson";
$RemapGroup[$RemapCount] = "Miscellaneous";
$RemapCount++;
$RemapName[$RemapCount] = "Toggle Camera";
$RemapCmd[$RemapCount] = "toggleCamera";
$RemapGroup[$RemapCount] = "Miscellaneous";
$RemapCount++;

function ControlsMenu::onWake(%this)
{
    %this.settingsPage = 0;
    %this.changeSettingsPage();
}

function ControlsMenu::changeSettingsPage(%this)
{
    ControlsMenuOptionsArray.clear();
    
    %lastPageIndex = 2;

    if(%this.settingsPage < 0)
        %this.settingsPage = 0;
        
    else if(%this.settingsPage > %lastPageIndex)
        %this.settingsPage = %lastPageIndex;

    if(%this.settingsPage == 0)
    {
        ControlsMenuPageText.setText("Movement");
        ControlsMenu.loadGroupKeybinds("Movement");
    }
    else if(%this.settingsPage == 1)
    {
        ControlsMenuPageText.setText("Combat");
        ControlsMenu.loadGroupKeybinds("Combat");
    }
    else if(%this.settingsPage == 2)
    {
        ControlsMenuPageText.setText("Miscellaneous");
        ControlsMenu.loadGroupKeybinds("Miscellaneous");
    }

    ControlsMenuOptionsArray.refresh();
}

function ControlsMenu::loadGroupKeybinds(%this, %keybindGroup)
{
   for(%i=0; %i < $RemapCount; %i++)
   {
      //find and add all the keybinds for the particular group we're looking at
      if($RemapGroup[%i] $= %keybindGroup)
      {
         %temp = %this.getKeybindString(%i);
         
         %option = %this.addKeybindOption();
         %option-->nameText.setText($RemapName[%i]);
         %option-->rebindButton.setText(%temp);
         %option-->rebindButton.keybindIndex = %i;
      }
   }
}

function ControlsMenu::addKeybindOption(%this)
{
    %graphicsOption = OptionsMenu.tamlReader.read("gui/ControlsMenuSetting.taml");

    ControlsMenuOptionsArray.add(%graphicsOption);

    return %graphicsOption;
}

function ControlsMenu::getKeybindString(%this, %index )
{
   %name       = $RemapName[%index];
   %cmd        = $RemapCmd[%index];

   %temp = moveMap.getBinding( %cmd );
   if ( %temp $= "" )
      return %name TAB "";

   %mapString = "";

   %count = getFieldCount( %temp );
   for ( %i = 0; %i < %count; %i += 2 )
   {
      if ( %mapString !$= "" )
         %mapString = %mapString @ ", ";

      %device = getField( %temp, %i + 0 );
      %object = getField( %temp, %i + 1 );
      %mapString = %mapString @ %this.getMapDisplayName( %device, %object );
   }

   return %mapString; 
}

function ControlsMenu::redoMapping( %device, %action, %cmd, %oldIndex, %newIndex )
{
	//%actionMap.bind( %device, %action, $RemapCmd[%newIndex] );
	moveMap.bind( %device, %action, %cmd );
	
   %remapList = %this-->OptRemapList;
	%remapList.setRowById( %oldIndex, buildFullMapString( %oldIndex ) );
	%remapList.setRowById( %newIndex, buildFullMapString( %newIndex ) );
}

function ControlsMenu::restoreDefaultMappings(%this)
{
   moveMap.delete();
   exec( "scripts/client/default.bind.cs" );
   %this.fillRemapList();
}

function ControlsMenu::getMapDisplayName( %this, %device, %action )
{
	if ( %device $= "keyboard" )
		return( %action );		
	else if ( strstr( %device, "mouse" ) != -1 )
	{
		// Substitute "mouse" for "button" in the action string:
		%pos = strstr( %action, "button" );
		if ( %pos != -1 )
		{
			%mods = getSubStr( %action, 0, %pos );
			%object = getSubStr( %action, %pos, 1000 );
			%instance = getSubStr( %object, strlen( "button" ), 1000 );
			return( %mods @ "mouse" @ ( %instance + 1 ) );
		}
		else
			error( "Mouse input object other than button passed to getDisplayMapName!" );
	}
	else if ( strstr( %device, "joystick" ) != -1 )
	{
		// Substitute "joystick" for "button" in the action string:
		%pos = strstr( %action, "button" );
		if ( %pos != -1 )
		{
			%mods = getSubStr( %action, 0, %pos );
			%object = getSubStr( %action, %pos, 1000 );
			%instance = getSubStr( %object, strlen( "button" ), 1000 );
			return( %mods @ "joystick" @ ( %instance + 1 ) );
		}
		else
	   { 
	      %pos = strstr( %action, "pov" );
         if ( %pos != -1 )
         {
            %wordCount = getWordCount( %action );
            %mods = %wordCount > 1 ? getWords( %action, 0, %wordCount - 2 ) @ " " : "";
            %object = getWord( %action, %wordCount - 1 );
            switch$ ( %object )
            {
               case "upov":   %object = "POV1 up";
               case "dpov":   %object = "POV1 down";
               case "lpov":   %object = "POV1 left";
               case "rpov":   %object = "POV1 right";
               case "upov2":  %object = "POV2 up";
               case "dpov2":  %object = "POV2 down";
               case "lpov2":  %object = "POV2 left";
               case "rpov2":  %object = "POV2 right";
               default:       %object = "??";
            }
            return( %mods @ %object );
         }
         else
            error( "Unsupported Joystick input object passed to getDisplayMapName!" );
      }
	}
		
	return( "??" );		
}

function ControlsMenu::buildFullMapString( %this, %index )
{
   %name       = $RemapName[%index];
   %cmd        = $RemapCmd[%index];

   %temp = moveMap.getBinding( %cmd );
   if ( %temp $= "" )
      return %name TAB "";

   %mapString = "";

   %count = getFieldCount( %temp );
   for ( %i = 0; %i < %count; %i += 2 )
   {
      if ( %mapString !$= "" )
         %mapString = %mapString @ ", ";

      %device = getField( %temp, %i + 0 );
      %object = getField( %temp, %i + 1 );
      %mapString = %mapString @ %this.getMapDisplayName( %device, %object );
   }

   return %name TAB %mapString; 
}

function ControlsMenu::fillRemapList( %this )
{
   %remapList = %this-->OptRemapList;
   
	%remapList.clear();
   for ( %i = 0; %i < $RemapCount; %i++ )
      %remapList.addRow( %i, %this.buildFullMapString( %i ) );
}

function ControlsMenu::doRemap( %this )
{
   %remapList = %this-->OptRemapList;
   
	%selId = %remapList.getSelectedId();
   %name = $RemapName[%selId];

	RemapDlg-->OptRemapText.setValue( "Re-bind \"" @ %name @ "\" to..." );
	OptRemapInputCtrl.index = %selId;
	Canvas.pushDialog( RemapDlg );
}

function ControlsMenuRebindButton::onClick(%this)
{
   %name = $RemapName[%i];
   RemapDlg-->OptRemapText.setValue( "Re-bind \"" @ %name @ "\" to..." );
   
   OptRemapInputCtrl.index = %this.keybindIndex;
   Canvas.pushDialog( RemapDlg );
}

function OptRemapInputCtrl::onInputEvent( %this, %device, %action )
{
   //error( "** onInputEvent called - device = " @ %device @ ", action = " @ %action @ " **" );
   Canvas.popDialog( RemapDlg );

   // Test for the reserved keystrokes:
   if ( %device $= "keyboard" )
   {
      // Cancel...
      if ( %action $= "escape" )
      {
         // Do nothing...
         return;
      }
   }

   %cmd  = $RemapCmd[%this.index];
   %name = $RemapName[%this.index];

   // Grab the friendly display name for this action
   // which we'll use when prompting the user below.
   %mapName = ControlsMenu.getMapDisplayName( %device, %action );
   
   // Get the current command this action is mapped to.
   %prevMap = moveMap.getCommand( %device, %action );

   // If nothing was mapped to the previous command 
   // mapping then it's easy... just bind it.
   if ( %prevMap $= "" )
   {
      ControlsMenu.unbindExtraActions( %cmd, 1 );
      moveMap.bind( %device, %action, %cmd );
      
      ControlsMenu.loadGroupKeybinds();
      //ControlsMenu-->OptRemapList.setRowById( %this.index, ControlsMenu.buildFullMapString( %this.index ) );
      return;
   }

   // If the previous command is the same as the 
   // current then they hit the same input as what
   // was already assigned.
   if ( %prevMap $= %cmd )
   {
      ControlsMenu.unbindExtraActions( %cmd, 0 );
      moveMap.bind( %device, %action, %cmd );

      ControlsMenu.loadGroupKeybinds();
      //ControlsMenu-->OptRemapList.setRowById( %this.index, ControlsMenu.buildFullMapString( %this.index ) );
      return;   
   }

   // Look for the index of the previous mapping.
   %prevMapIndex = ControlsMenu.findRemapCmdIndex( %prevMap );
   
   // If we get a negative index then the previous 
   // mapping was to an item that isn't included in
   // the mapping list... so we cannot unmap it.
   if ( %prevMapIndex == -1 )
   {
      MessageBoxOK( "Remap Failed", "\"" @ %mapName @ "\" is already bound to a non-remappable command!" );
      return;
   }

   // Setup the forced remapping callback command.
   %callback = "redoMapping(" @ %device @ ", \"" @ %action @ "\", \"" @
                              %cmd @ "\", " @ %prevMapIndex @ ", " @ %this.index @ ");";
   
   // Warn that we're about to remove the old mapping and
   // replace it with another.
   %prevCmdName = $RemapName[%prevMapIndex];
   Canvas.pushDialog( RemapConfirmDlg );
   
   RemapConfirmationText.setText("\"" @ %mapName @ "\" is already bound to \""
      @ %prevCmdName @ "\"!\nDo you wish to replace this mapping?");
   RemapConfirmationYesButton.command = "ControlsMenu.redoMapping(" @ %device @ ", \"" @ %action @ "\", \"" @
                              %cmd @ "\", " @ %prevMapIndex @ ", " @ %this.index @ "); Canvas.popDialog();";
   RemapConfirmationNoButton.command = "Canvas.popDialog();";
   
   /*MessageBoxYesNo( "Warning",
      "\"" @ %mapName @ "\" is already bound to \""
      @ %prevCmdName @ "\"!\nDo you wish to replace this mapping?",
       %callback, "" );*/
}

function ControlsMenu::findRemapCmdIndex( %this, %command )
{
	for ( %i = 0; %i < $RemapCount; %i++ )
	{
		if ( %command $= $RemapCmd[%i] )
			return( %i );			
	}
	return( -1 );	
}

/// This unbinds actions beyond %count associated to the
/// particular moveMap %commmand.
function ControlsMenu::unbindExtraActions( %this, %command, %count )
{
   %temp = moveMap.getBinding( %command );
   if ( %temp $= "" )
      return;

   %count = getFieldCount( %temp ) - ( %count * 2 );
   for ( %i = 0; %i < %count; %i += 2 )
   {
      %device = getField( %temp, %i + 0 );
      %action = getField( %temp, %i + 1 );
      
      moveMap.unbind( %device, %action );
   }
}

function ControlsMenu::redoMapping( %this, %device, %action, %cmd, %oldIndex, %newIndex )
{
	//%actionMap.bind( %device, %action, $RemapCmd[%newIndex] );
	moveMap.bind( %device, %action, %cmd );
	
   %remapList = %this-->OptRemapList;
	%remapList.setRowById( %oldIndex, %this.buildFullMapString( %oldIndex ) );
	%remapList.setRowById( %newIndex, %this.buildFullMapString( %newIndex ) );
	
	%this.changeSettingsPage();
}