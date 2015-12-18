// Display a splash window immediately to improve app responsiveness before
// engine is initialized and main window created.
displaySplashWindow("data/art/splash.bmp");

// Console does something.
setLogMode(1);

// Disable script trace.
trace(false);

// Set the name of our application
$appName = "New Template";

//-----------------------------------------------------------------------------
// Load up scripts to initialise subsystems.
exec("core/main.cs");

// Parse the command line arguments
echo("\n--------- Parsing Arguments ---------");
parseArgs();

// The canvas needs to be initialized before any gui scripts are run since
// some of the controls assume that the canvas exists at load time.
createCanvas($appName);

//-----------------------------------------------------------------------------
// Load console.
exec("core/console/main.cs");

// Init the physics plugin.
physicsInit();
  
// Start up the audio system.
sfxStartup();

// Set up networking.
setNetPort(0);

// Start processing file change events.   
startFileChangeNotifications();

// If we have editors, initialize them here as well
if(isFile("tools/main.cs") && !$isDedicated)
	exec("tools/main.cs");
	
ModuleDatabase.setModuleExtension("module");
ModuleDatabase.scanModules( "data", true );
ModuleDatabase.LoadExplicit( "Game" );

if( !$isDedicated )
{
   // Start rendering and stuff.
   initRenderManager();
   initLightingSystems("Advanced Lighting"); 

   // Start PostFX. If you use "Advanced Lighting" above, uncomment this.
   initPostEffects();
   
   closeSplashWindow();
   
   // As we know at this point that the initial load is complete,
   // we can hide any splash screen we have, and show the canvas.
   // This keeps things looking nice, instead of having a blank window
   Canvas.showWindow();
}
else
{
   closeSplashWindow();
}

echo("Engine initialized...");

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onExit() 
{
   // Stop file change events.
   stopFileChangeNotifications();
   
   ModuleDatabase.UnloadExplicit( "Game" );
}