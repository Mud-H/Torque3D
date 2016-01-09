function initClient()
{
   echo("\n--------- Initializing " @ $appName @ ": Client Scripts ---------");

   // Make sure this variable reflects the correct state.
   $Server::Dedicated = false;

   // Game information used to query the master server
   $Client::GameTypeQuery = $appName;
   $Client::MissionTypeQuery = "Any";
   
   exec( "data/clientServer/scripts/client/connectionToServer.cs" );
   exec( "data/clientServer/scripts/client/levelDownload.cs" );
   exec( "data/clientServer/scripts/client/levelLoad.cs" );
   exec( "data/clientServer/scripts/client/message.cs" );
   
   //load prefs
   if ( isFile( "data/clientServer/scripts/client/prefs.cs" ) )
      exec( "data/clientServer/scripts/client/prefs.cs" );
   else
      exec( "data/clientServer/scripts/client/defaults.cs" );

   loadMaterials();

   // Copy saved script prefs into C++ code.
   setDefaultFov( $pref::Player::defaultFov );
   setZoomSpeed( $pref::Player::zoomSpeed );
}