In order to use mov files with alpha in chrome, you need vp9 with alpha support.  We can convert mov with alpha with following commands:

ffmpeg -i EnergyExplosion22.mov  -c:v libvpx-vp9 -pix_fmt yuva420p EE22.webm

we can then set the videoplayer url to /StreamingAssets/*.webm

-enable data cache for the build to ensure every run doesnt make web request

-ensure that the videoplayer material shader is set to unlit texture (https://forum.unity.com/threads/new-unity-video-player-in-webgl.480383/)



