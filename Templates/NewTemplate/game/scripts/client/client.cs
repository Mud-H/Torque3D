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
   if ( isFile( "scripts/client/prefs.cs" ) )
      exec( "scripts/client/prefs.cs" );
   else
      exec( "scripts/client/defaults.cs" );
   
   //Load UI stuff
   //Profiles first
   exec("scripts/gui/profiles.cs");
   
   //Now gui files
   exec("gui/mainMenu.gui");
   exec("gui/chooseLevelDlg.gui");
   exec("gui/joinServerMenu.gui");
   exec("gui/loadingGui.gui");
   //exec("gui/messageBoxOK.ed.gui");
   exec("gui/messageBoxYesNo.gui");
   exec("gui/optionsMenu.gui");
   exec("gui/playGui.gui");
   exec("gui/pauseMenu.gui");
   exec("gui/remapDlg.gui");
   exec("gui/remapConfirmDlg.gui");
   
   exec("gui/profiler.gui");
   
   //Load gui companion scripts
   exec("scripts/gui/chooseLevelDlg.cs");
   exec("scripts/gui/messageBoxes.cs");
   exec("scripts/gui/optionsList.cs");
   exec("scripts/gui/optionsMenu.cs");
   exec("scripts/gui/playGui.cs");
   exec("scripts/gui/chooseLevelDlg.cs");
   exec("scripts/gui/mainMenu.cs");
   exec("scripts/gui/joinServerMenu.cs");
   exec("scripts/gui/pauseMenu.cs");
   
   //Keybinds and input scripts
   exec("scripts/client/game/keybinds.cs");
   exec("scripts/client/game/inputCommands.cs");
   
   //client system scripts
   exec("scripts/client/message.cs");
   exec("scripts/client/levelDownload.cs");
   exec("scripts/client/levelLoad.cs");
   exec("scripts/client/connectionToServer.cs");
   
   // Initialize the post effect manager.
   exec("core/postFx/postFXManager.gui");
   exec("core/postFx/postFXManager.gui.cs");
   exec("core/postFx/postFXManager.gui.settings.cs");
   exec("core/postFx/postFXManager.persistance.cs");
   
   // Initialize any game-specific postFX here
   exec("scripts/client/postfx/chromaticLens.cs");
   exec("scripts/client/postfx/dof.cs");
   exec("scripts/client/postfx/flash.cs");
   exec("scripts/client/postfx/lightRay.cs");
   exec("scripts/client/postfx/MotionBlurFx.cs");
   exec("scripts/client/postfx/vignette.cs");
   
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