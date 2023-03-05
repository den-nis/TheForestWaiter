# TheForestWaiter
The Forest Waiter is a platformer made using SFML. This project initially started as a joke, but we decided to finish it. The name is a Dutch word "boswachter" poorly translated to "Forest Waiter".

[![Build for windows](https://github.com/den-nis/TheForestWaiter/actions/workflows/BuildWindows.yml/badge.svg)](https://github.com/den-nis/TheForestWaiter/actions/workflows/BuildWindows.yml)

## Controls
The following table contains the default controls. Some of these controls can be adjusted by editing the `Settings.ini` file.

Key | Action 
---|---
A | Move left
D | Move right
E | Toggle the shop hud
Space | Jump
Left mouse | Shoot
F11 | Fullscreen
Number keys | Switch between items

## Credits

All of the graphics have been made by [Joost](https://github.com/BigJosso).
The majority of the programming is done by me, besides for two source files.

`TheForestWaiter/Game/Particles/ParticleSystem.cs` Contains code roughly based on [Cherno's particle system](https://github.com/TheCherno/OneHourParticleSystem).

`TheForestWaiter/Game/Essentials/Collisions.cs` Has a method called SweptAABB based on [www.gamedev.net](https://www.gamedev.net/articles/programming/general-and-gameplay-programming/swept-aabb-collision-detection-and-response-r3084/).



## Screenshots
![a](https://user-images.githubusercontent.com/50838791/182452988-27835e5a-bf1c-450b-95d0-c41c7fd465c6.png)
![e](https://user-images.githubusercontent.com/50838791/182453024-e20728c0-2fe4-4208-a9df-4ee6be67e244.png)
![f](https://user-images.githubusercontent.com/50838791/182453026-04893d96-3b97-4fd5-8b38-b9cab740ba7c.png)
![g](https://user-images.githubusercontent.com/50838791/182453028-92e65c5f-dc8c-47e0-8d4d-48600b71c134.png)
![h](https://user-images.githubusercontent.com/50838791/182453031-9adf42da-db14-4fc5-9256-1f2c37a9f312.png)
![i](https://user-images.githubusercontent.com/50838791/182453039-c1f94d45-be1f-4cd2-9e1a-f5756da8b01e.png)

## Project files
### TheForestWaiter
This is the main project and will compile the game.
### TheForestWaiter.Content
This is a console application that is used to pre-process content used by the game. It will for example compress the world map. The output is a zip file which will be embedded in the game executable. The main project runs this application using a pre-build event.
### TheForestWaiter.Shared
This is a class library for shared classes between `TheForestWaiter` and `TheForestWaiter.Content`.
