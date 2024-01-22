# FPS-Demo Documentation
## Project Overview
This is a simple horror FPS game based on Low Poly style models. It provides several weapons that can be picked up and has designed shooting and enemy systems. It's created using the `C#` language and `Unity3D Build-In` rendering pipeline.  

**Note: FPS-Demo is for learning and reference only. It may lack certain entertainment and fun elements and is not intended for commercial use. We do not take any responsibility for the stability and final results of the project.**

## Language
The default README language for FPS-Demo projects is English, if you want to choose another language, please select it below:
- [FPS-Demo中文文档](README_ZH.md)

## Table of Contents
If you only want to experience the project simply, you can refer to the first and second parts without understanding the underlying principles. If you are learning C# or Unity, we recommend reading our entire documentation to familiarize yourself with the operation logic and mechanisms of FPS-Demo for reference and learning.
#### Part One - [Download and Usage](#section1)
#### Part Two - [Control Modes](#section2)
#### Part Three - [Implementation of Firearms](#section3)
#### Part Four - [Implementation of Enemies](#section4)
#### Part Five - [Scenes](#section5)

<h2 id="section1">Download and Usage</h2>

### Download
Generally, we recommend users to directly go to the `Releases` page to download the packaged game software corresponding to the operating version, instead of directly downloading the source code in `Code - DownloadZip`.

### aunch
FPS-Demo has different installation methods on different operating systems. Please follow the instructions below after downloading the software for your architecture:
#### MacOS
If you are using MacOS, you can directly move the downloaded software to `Finder-Sidebar-Applications` or `Users/(Your Username)/Application`. If all goes well, you will see it in the Launchpad. Clicking the left mouse button will open it, and you can force exit the game by clicking `Esc - Quit Game` in the game or pressing `[LeftCommand] + [Q]` on any interface.
#### Windows
If you are using Windows, you will typically receive a folder. If you want to launch the program directly, you can enter the folder and open `FPS-Demo.exe`. For a better launch experience, you can manually move the folder to the default download folder or a custom path, i.e., `C:\Program Files\`, and add a shortcut to the desktop.

<h2 id="section2">Control Modes</h2>

### Observer Mode
In Observer Mode, use `WASD` for planar movement, and move the mouse to change the viewpoint (with a maximum angle limit for up and down angles). The movement of `WASD` is only affected by the rotation caused by the left and right movement of the mouse, meaning WASD can only achieve planar movement and cannot move on the Y-axis. You can operate the observer camera with the following keys:
   - Use the `[E]` key for positive movement on the Y-axis.
   - Use the `[Q]` key for negative movement on the Y-axis.  
 
**Note: The observer camera has no collision and is not affected by gravity, so it is not influenced by scene collision objects.**

### Character Movement
FPS-Demo uses the classic WASD for planar movement. The following keys perform operations on the player:
   - Use the `[SPACE]` key to jump.
   - Use the `[LeftShift]` key to run and increase speed.
   - Use the `[Control]` key to crouch, reducing movement noise and speed.

### Weapon Operation
All weapons in FPS-Demo use the same operation logic. Shooting is achieved by clicking the left mouse button. For automatic weapons, holding down the left mouse button allows continuous shooting. Zooming in with the weapon is done by clicking the right mouse button; click once to zoom in, click again to zoom out. The following keys control the weapon:
   - Press `[I]` to inspect.
   - Press `[R]` to reload.
   - Press `[C]` to toggle the flashlight on and off.
   - Press `[Q]` for melee attack.
   - Press `[X]` to switch firing mode (only effective for automatic weapons).

### Weapon Inventory Operation
The weapon inventory in FPS-Demo has a maximum of 3 weapons. When there are already 3 weapons in the inventory, no more can be picked up. Weapon switching is determined by the mouse wheel or the numeric keypad. When using the mouse wheel to switch weapons and it's already the last weapon, scrolling the mouse wheel down will switch to the first weapon. Using the numeric keypad to switch weapons, if the input is 0 or exceeds the number of weapons in the inventory, it will switch to unarmed. To pick up a weapon, just move to the weapon's position, and it will be automatically picked up and switched to the current weapon. Press `[T]` to discard the weapon; it will be dropped in front of the player and can be picked up again.

### Attacking Enemies
In FPS-Demo, you can attack enemies with weapons and interactive objects in the scene. The judgment for causing damage to enemies with firearms uses Raycast based on physics. Different models and types of firearms cause different damage to enemies. Similarly, most interactive objects in the scene, such as oil drums and gas cylinders, can also damage enemies. Bullets or melee can cause them to explode. Different types of objects cause different damage and have different damage ranges to enemies. For interactive objects in the scene, see `/Scene - Interactive Objects/`.

<h2 id="section3">Implementation of Firearms</h2>

### Implementation of Firearms
All firearms in FPS-Demo are rendered separately from the scene, overlaying the depth view of the Player layer rendered by GunCamera on top of MainCamera. Thus, all firearms in FPS-Demo can never penetrate through scene objects.

### Shooting Judgment
FPS-Demo uses two systems for judgment. For hit detection and damage deduction on enemies, it uses Physics.Raycast (ray detection) emitted from the gun muzzle. For the generation of bullet holes and interactive objects in the scene (such as oil drums, gas cylinders), and bullet holes, it uses real bullets based on Rigidbody physics, applying initial velocity to the bullets, and uses geometric node-based collision to determine impacts.

### Reloading of Firearms
For all rifles, pistols, and some sniper rifles with magazines, the bullets are instantly added when inserting the magazine. For all shotguns and some sniper rifles, the addition of bullets is according to the execution of the animation in the animation state machine, meaning the actual number of bullets naturally increases according to the animation of loading the bullets one by one.

###  Implementation of Zoom-in Effect Inside the Scope
All sniper rifles in FPS-Demo map the image to the front of the sniper scope in space using a special low FOV camera to achieve the zoom-in effect. The sniper scope will display different colors on the front glass when zooming in and out to simulate the real effect.

### Dropping and Picking Up Weapons
FPS-Demo adds a Trigger collision box to the firearm models so that firearms can be automatically picked up. When discarding a weapon, the project uses the direct generation method, using a MeshCollider and Rigidbody based on physics to make the firearm object naturally fall to the ground.

<h2 id="section4">Implementation of Enemies</h2>

### Overview of Enemies
There are currently only two types of enemies in FPS-Demo, Zombie and Mutant. Mutants are far stronger than zombies in terms of health, speed, and damage. Therefore, theoretically, zombies usually operate in groups under the leadership of mutants. Generally, enemies should patrol at walking speed when they have not discovered the player and run towards the player after spotting them. Of course, it's not uncommon for zombies or mutants to appear or even patrol alone.

### Enemy Detection of Players
Enemies will only detect players under certain conditions, as follows:
```swift
// Pseudocode
if(player enters the enemy's detection range){
    if(player is not crouching && player is moving) enemy detects player;
    if(player uses a gun without a silencer to shoot) enemy detects player;
    if(player reaches the enemy's absolute detection threshold) enemy detects player;
}else{
    the enemy will not detect the player;
}

``` 
**Note: Different enemies have different detection ranges and absolute detection thresholds.**

### Enemy Attacks
Enemies will stop and initiate attack routines when they enter the attack range. Different types of enemies have different attack damages and cooldowns. For zombies, the enemy will have a shorter cooldown and lesser damage; for mutants, the enemy will have a longer cooldown and significantly more damage, and mutants will have explosive particle effects when attacking. Players in the enemy's attack range and when the enemy is on cooldown will perform an Idle animation.

<h2 id="section5">Scenes</h2>

### Interactive Objects
Only oil drums and gas cylinders in the FPS-Demo scene can interact with bullets and players. Oil drums and gas cylinders can be pushed by players. The explosion judgment of interactive objects is executed by physically launched bullets. When a bullet collides with an oil drum, the drum will explode instantly, releasing a large amount of damage over a large area. When a bullet collides with a gas cylinder, the cylinder will leak gas from the top, and when the internal pressure reaches a threshold, it will explode, releasing a certain amount of damage over a relatively small area.  
  
**Note: The destruction of interactive objects in the scene will not affect the terrain.**

### Special Scenes
Some specific scenes in FPS-Demo will have certain effects on the player. For example, in water, players will be forced to crouch, cannot jump, and will play the sound of moving in water when moving. For all scenes entered by the player and at this time the height of the ceiling is less than the standing height of the player, the player will be forced to crouch. Even if the user releases `[Control]`, as long as the user does not press `[Control]` after leaving the scene, the player will automatically re-enter the standing state.