# That is how you wish a gamer happy bithday

I have a friend (have to ask her on that :fearful: )
which I wanted to wish happy birthsday to, in a unique way so I knew she sometimes plays Euro Truck Simulator 2 (This should also work on American Truck Simulator, although I didn't test it) and I am familiar with that game (actually playing it) so why not write a nice message to her in the game
## How to use
1. Download game SDK from here: https://github.com/RenCloud/scs-sdk-plugin/releases/download/V.1.10.5/release_v_1_10_5.zip
2. Extraxt the zip and copy stuff into your game/plugins directory

**for x64 systems copy** everything from Win64 folder into 
```C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2\bin\win_x64\plugins``` (create the folder if it doesn't exists yet)

**for x86 systems copy** everything from Win32 folder into 
```C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2\bin\win_x32\plugins``` (create the folder if it doesn't exists yet)

3. Run ETS2-MeM.exe
4. Enable Developer options (if you want to change the time, or wait til 0:00)
5. Launch Euro Truck Simulator 2 and start the game
6. Wait til 00:00 (12AM) or use console and type g_set_time 0 00 to set the time to midnight

## Costumization 
If you want to costumize this, then edit ```ETS2-MeM.exe.Config```
and change 
```xml
<add key="HappyText" value="Your white colored thext" />
<add key="BirthdayText" value="Your Yellow colored text" />
<add key="CakeImage" value="YourImage.png" />
```

## How was this done
using [SCS-SDK](https://github.com/RenCloud/scs-sdk-plugin) Plugin to get game time 
using [Direct3DHook](https://github.com/spazzarama/Direct3DHook) to be able to hook into DirectX Video and draw on screen

Borrowed some code from [ets2-local-radio](https://github.com/Koenvh1/ets2-local-radio)
as I first saw the possibility to draw on screen right in this project


