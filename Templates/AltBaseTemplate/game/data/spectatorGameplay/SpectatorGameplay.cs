
// The general flow of a gane - server's creation, loading and hosting clients, and then destruction is as follows:

// First, a client will always create a server in the event that they want to host a single player
// game. Torque3D treats even single player connections as a soft multiplayer game, with some stuff
// in the networking short-circuited to sidestep around lag and packet transmission times.

// initServer() is called, loading the default server scripts.
// After that, if this is a dedicated server session, initDedicated() is called, otherwise initClient is called
// to prep a playable client session.

// When a local game is started - a listen server - via calling StartGame() a server is created and then the client is
// connected to it via createAndConnectToLocalServer().

function SpectatorGameplay::create( %this )
{
   //server scripts
   exec("./scripts/server/camera.cs");
   exec("./scripts/server/DefaultGame.cs");
   
   //add DBs
   if(isObject(DatablockFilesList))
   {
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/camera.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/defaultParticle.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/lights.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/managedDatablocks.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/managedDecalData.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/managedForestItemData.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/managedParticleData.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/managedParticleEmitterData.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/markers.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/ribbons.cs" );
      DatablockFilesList.add( "data/spectatorGameplay/scripts/datablocks/sounds.cs" );
   }
   
   if (!$Server::Dedicated)
   {
      //client scripts
      exec("./scripts/client/default.keybinds.cs");
      exec("./scripts/client/keybinds.cs");
      exec("./scripts/client/inputCommands.cs");
      
      //guis
      exec("./scripts/gui/playGui.gui");
      exec("./scripts/gui/playGui.cs");
      
      //postFX stuffs
      exec("./scripts/gui/postFxManager.gui");
      
      exec("./scripts/client/postFX/postFxManager.gui.cs");
      exec("./scripts/client/postFX/postFxManager.gui.settings.cs");
      exec("./scripts/client/postFX/postFxManager.persistance.cs");
      
      exec("./scripts/client/postFX/default.postfxpreset.cs");
      
      exec("./scripts/client/postFX/caustics.cs");
      exec("./scripts/client/postFX/chromaticLens.cs");
      exec("./scripts/client/postFX/dof.cs");
      exec("./scripts/client/postFX/edgeAA.cs");
      exec("./scripts/client/postFX/flash.cs");
      exec("./scripts/client/postFX/fog.cs");
      exec("./scripts/client/postFX/fxaa.cs");
      exec("./scripts/client/postFX/GammaPostFX.cs");
      exec("./scripts/client/postFX/glow.cs");
      exec("./scripts/client/postFX/hdr.cs");
      exec("./scripts/client/postFX/lightRay.cs");
      exec("./scripts/client/postFX/MLAA.cs");
      exec("./scripts/client/postFX/MotionBlurFx.cs");
      exec("./scripts/client/postFX/ovrBarrelDistortion.cs");
      exec("./scripts/client/postFX/ssao.cs");
      exec("./scripts/client/postFX/turbulence.cs");
      exec("./scripts/client/postFX/vignette.cs");
   }
}

function SpectatorGameplay::destroy( %this )
{
   
}