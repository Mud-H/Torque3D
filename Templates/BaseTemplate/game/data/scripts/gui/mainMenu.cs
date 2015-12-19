singleton SFXProfile(TestSound)
{
   fileName = "data/sound/cheetah_engine.ogg";
   Description = "Audio2D";
};

function MainMenuGui::onWake(%this)
{
   if (isFunction("getWebDeployment") &&
       getWebDeployment() &&
       isObject(%this-->ExitButton))
      %this-->ExitButton.setVisible(false);
      
   MainMenuButtonContainer.hidden = false;
}

function MainMenuGui::openSinglePlayerMenu(%this)
{
   $pref::HostMultiPlayer=false;
   Canvas.pushDialog(ChooseLevelDlg);
   ChooseLevelDlg.returnGui = %this; 
   MainMenuButtonContainer.hidden = true; 
}

function MainMenuGui::openMultiPlayerMenu(%this)
{
   $pref::HostMultiPlayer=true;
   Canvas.pushDialog(ChooseLevelDlg);
   ChooseLevelDlg.returnGui = %this; 
   MainMenuButtonContainer.hidden = true; 
}

function MainMenuGui::openOptionsMenu(%this)
{
   Canvas.pushDialog(OptionsMenu);
   OptionsMenu.returnGui = %this; 
   MainMenuButtonContainer.hidden = true; 
}

function MainMenuGui::onReturnTo(%this)
{
   MainMenuButtonContainer.hidden = false;
}