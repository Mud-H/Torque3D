function initClient()
{
   echo("\n--------- Initializing " @ $appName @ ": Client Scripts ---------");

   // Make sure this variable reflects the correct state.
   $Server::Dedicated = false;

   // Game information used to query the master server
   $Client::GameTypeQuery = $appName;
   $Client::MissionTypeQuery = "Any";

   // Use our prefs to configure our Canvas/Window
   configureCanvas();
   
   //load prefs
   if ( isFile( "data/scripts/client/prefs.cs" ) )
      exec( "data/scripts/client/prefs.cs" );
   else
      exec( "data/scripts/client/defaults.cs" );
      
   // Start up the audio system.
   sfxStartup();
      
   //Load UI stuff
   //we need to load this because some of the menu profiles use the sounds here
   exec("data/scripts/datablocks/guiSounds.cs");
   
   //Profiles
   exec("data/scripts/gui/profiles.cs");
   
   //Now gui files
   exec("data/gui/mainMenu.gui");
   exec("data/gui/chooseLevelDlg.gui");
   exec("data/gui/joinServerMenu.gui");
   exec("data/gui/loadingGui.gui");
   exec("data/gui/messageBoxYesNo.gui");
   exec("data/gui/optionsMenu.gui");
   exec("data/gui/playGui.gui");
   exec("data/gui/pauseMenu.gui");
   exec("data/gui/remapDlg.gui");
   exec("data/gui/remapConfirmDlg.gui");
   
   exec("data/gui/profiler.gui");
   
   //Load gui companion scripts
   exec("data/scripts/gui/chooseLevelDlg.cs");
   exec("data/scripts/gui/messageBoxes.cs");
   exec("data/scripts/gui/optionsList.cs");
   exec("data/scripts/gui/optionsMenu.cs");
   exec("data/scripts/gui/playGui.cs");
   exec("data/scripts/gui/chooseLevelDlg.cs");
   exec("data/scripts/gui/mainMenu.cs");
   exec("data/scripts/gui/joinServerMenu.cs");
   exec("data/scripts/gui/pauseMenu.cs");
   
   //Keybinds and input scripts
   exec("data/scripts/client/game/keybinds.cs");
   exec("data/scripts/client/game/inputCommands.cs");
   
   //client system scripts
   exec("data/scripts/client/message.cs");
   exec("data/scripts/client/levelDownload.cs");
   exec("data/scripts/client/levelLoad.cs");
   exec("data/scripts/client/connectionToServer.cs");
   
   // Initialize the post effect manager.
   exec("core/postFx/postFXManager.gui");
   exec("core/postFx/postFXManager.gui.cs");
   exec("core/postFx/postFXManager.gui.settings.cs");
   exec("core/postFx/postFXManager.persistance.cs");
   
   // Initialize any game-specific postFX here
   exec("data/scripts/client/postfx/chromaticLens.cs");
   exec("data/scripts/client/postfx/dof.cs");
   exec("data/scripts/client/postfx/flash.cs");
   exec("data/scripts/client/postfx/lightRay.cs");
   exec("data/scripts/client/postfx/MotionBlurFx.cs");
   exec("data/scripts/client/postfx/vignette.cs");
   
   //Autodetect settings if it's our first time
   if($pref::Video::autoDetect)
      GraphicsMenu.Autodetect();
   
   loadMaterials();
   
   // Really shouldn't be starting the networking unless we are
   // going to connect to a remote server, or host a multi-player
   // game.
   setNetPort(0);

   // Copy saved script prefs into C++ code.
   setDefaultFov( $pref::Player::defaultFov );
   setZoomSpeed( $pref::Player::zoomSpeed );

   // Start up the main menu... this is separated out into a
   // method for easier mod override.

   if ($startWorldEditor || $startGUIEditor) {
      // Editor GUI's will start up in the primary main.cs once
      // engine is initialized.
      return;
   }

   // Connect to server if requested.
   if ($JoinGameAddress !$= "") {
      // If we are instantly connecting to an address, load the
      // loading GUI then attempt the connect.
      loadLoadingGui();
      connect($JoinGameAddress, "", $Pref::Player::Name);
   }
   else 
   {
      // Otherwise go to the splash screen.
      Canvas.setCursor(DefaultCursor);
      Canvas.pushDialog(MainMenuGui);
   }   
}