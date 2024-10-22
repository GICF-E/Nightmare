# Nightmare English Documentation
## Project Overview
This is a simple horror FPS game. It provides several weapons that can be picked up and has designed shooting and enemy systems. It's created using the `C#` language and `Unity3D Build-In` rendering pipeline.Please click [here](https://www.bilibili.com/video/BV1kz421Q7Zs/?spm_id_from=333.999.0.0&vd_source=0fe56d06a40b2ce75fc37997448a105c) to watch the promotional video.

**Note: This project is for learning and reference only. It may lack certain entertainment and fun elements and is not intended for commercial use. We do not take any responsibility for the stability and final results of the project.**

## Language
The default README language for Nightmare projects is English, if you want to choose another language, please select it below:
   - [Nightmare 中文文档](README_ZH.md)

## Table of Contents
If you only want to experience the project simply, you can refer to the first and second parts without understanding the underlying principles. If you are learning C# or Unity, we recommend reading our entire documentation to familiarize yourself with the operation logic and mechanisms of Nightmare for reference and learning.
#### Part One - [Download and Usage](#section1)
#### Part Two - [Operation Mode](#section2)
#### Part Three - [Implementation of Firearms](#section3)
#### Part Four - [Implementation of Enemies](#section4)
#### Part Five - [Scene Implementation](#section5)
#### Part Six - [Archive System](#section6)
#### Part Seven - [Material Usage](#section7)

<h2 id="section1">Download and Usage</h2>

### Download
Generally, we recommend users to directly go to the `Releases` page to download the packaged game software corresponding to the operating version, instead of directly downloading the source code in `Code - DownloadZip`.

### Launch
Nightmare has different installation methods on different operating systems. Please follow the instructions below after downloading the software for your architecture:

#### MacOS
If you are using MacOS, you can directly move the downloaded software to `Finder-Sidebar-Applications` or `Users/(Your Username)/Application`. If all goes well, you will see it in the Launchpad. Clicking the left mouse button will open it, and you can force exit the game by clicking `Esc - Quit Game` in the game.

#### Windows
If you are using Windows, you will typically receive a folder. If you want to launch the program directly, you can enter the folder and open `Nightmare.exe`. For a better launch experience, you can manually move the folder to the default download folder or a custom path, i.e., `C:\Program Files\`, and add a shortcut to the desktop.

#### Linux
If you're using the Linux operating system, first ensure that the Unity game folder you've downloaded contains a Linux platform executable file. Typically, this file won't have a .exe extension but will be an executable file without any extension, and you might need to manually grant it execution permissions. Here's how to install and start the game: 
1. Grant Execution Permission: Open the terminal and use the cd command to navigate to the game folder's location. Then, use the chmod command to grant the main game executable file execution permissions. Assuming the main executable file is named Nightmare, you can use the following command:
   ```bash
   chmod +x Nightmare.x86_64
   ```
2. Start the Game: Once execution permission has been granted, you can start the game directly from the terminal by entering:
   ```bash
   ./Nightmare.x86_64
   ```
   If the game is located in a directory within your `PATH` environment variable, you can also start it by simply entering the game's name.

For convenience, you may also create a desktop shortcut or add the game to your application launcher, so you can start the game without going through the terminal. Different Linux distributions might have different methods for creating shortcuts, usually involving creating a `.desktop` file and specifying the appropriate game start command and path.

<h2 id="section2">Operation Mode</h2>

### Input Devices
Nightmare supports most keyboards, mice, and gamepads. Generally speaking, as long as the keyboard, mouse, and gamepad can function normally within the system, the game can automatically recognize all supported input devices. The Nightmare has the following requirements for input devices:
   - Keyboard: At least 60 keys, including basic function and auxiliary keys.
   - Mouse: Must include at least a left and right button, along with a scroll wheel.
   - Gamepad: It is recommended to use Xbox or PlayStation controllers. Other layouts have not been tested, and their feasibility and usability are not guaranteed.
 
***Note: Nightmare can connect multiple gamepads and keyboards simultaneously and supports simultaneous input. Once a gamepads is recognized, the game will automatically adjust the input sensitivity and aiming mode. Therefore, we do not recommend using a keyboard to play the game when a gamepad is connected.**

### Character Movement
Nightmare utilizes universally common and logical input key bindings. You can perform actions for the player using the following keys:
   - Use `[WASD]/[Left Stick]` for planar movement.
   - Press `[SPACE]/[Button West]` to jump.
   - Press `[Shift]/[Right Shoulder]` to run and increase speed.
   - Press `[Control]/[Command]/[Left Shoulder]` to crouch, reducing movement noise and speed.
   - Press `[Tap]` or `[Left Shoulder] & [Right Shoulder]` to roll.

### Weapon Operation
All weapons in Nightmare use a unified operating logic. Shooting is achieved through the left mouse button, and for fully automatic weapons, holding down the left mouse button allows for continuous firing. Aiming in Nightmare can be done by either clicking or holding down the right mouse button, and you can switch control methods in the settings. Control the weapons with the following keys:
   - Press `[I]/[Button North]` to inspect.
   - Press `[R]/[Button East]` to reload.
   - Press `[C]/[D-Pad Left]` to toggle the flashlight on and off.
   - Press `[Q]/[Button Sourth]` for melee attack.
   - Press `[X]/[D-Pad Right]` to switch firing mode (only effective for automatic weapons).

### Weapon Switching
In the Nightmare, the arsenal is managed through the mouse scroll wheel or the numeric keypad to change weapons. If using the mouse scroll wheel, you can switch to the next/previous weapon by scrolling down/up. If it's already the last weapon, the next scroll will switch to a bare-handed state, and scrolling again will switch back to the first weapon. Similarly, using the numeric keypad, you can directly switch to the weapon corresponding to the pressed key. If the input is out of range or zero, meaning the number does not correspond to any existing weapon, it will switch to the bare-handed state. The bare-handed state does not allow for melee attacks.

### Object Dropping and Picking Up
In Nightmare, if you want to pick up weapons or interactable objects, you need to move the player close to the object and move the view to the object you want to pick up/interact with. If the object can be interacted with, a semi-transparent circle will appear above it, indicating that you can perform the pickup operation. For firearms, if you don't already have the weapon, picking it up will automatically switch to it. If you already have the weapon, picking it up again will automatically refill the corresponding weapon's ammunition and switch to the current weapon. If you want to drop an object, you need to switch to the firearm you wish to discard and press the drop key. The object will be generated in front of you and fall naturally. You can interact with objects by pressing the following keys:
   - Press `[F]/[D-Pad Up]` to pick up/interact with objects
   - Press `[T]/[D-Pad Down]` to drop objects  

***Note: The weapon inventory is limited to 3, meaning if you already have 3 weapons, you will not be able to pick up a new one.**

### Health Regeneration
In Nightmare, the only way to regenerate health currently is by consuming food. If you want to regenerate health through food, you need to pick up the food (for the method, please refer to `/Operation Mode - Object Dropping and Picking Up/`). The character will automatically consume the food after you pick it up. During the consumption of the food, you will hear sound cues, and the health will continuously increase. Currently, the Nightmare supports a variety of foods, each with different amounts of health regeneration and chewing times.  
  
***Note: Note: Only one food item can be consumed at a time.**

### Attacking Enemies
In Nightmare, you can attack enemies with weapons and interactive objects in the scene. The judgment for causing damage to enemies with firearms uses Raycast based on physics. Different models and types of firearms cause different damage to enemies. Similarly, most interactive objects in the scene, such as oil drums and gas cylinders, can also damage enemies. Bullets or melee can cause them to explode. Different types of objects cause different damage and have different damage ranges to enemies. For interactive objects in the scene, see `/Scene Implementation - Interactive Objects/`.  
  
***Note: A bullet hitting the head can significantly increase damage，and bullets can push back enemies to some extent.**

### Viewing Notes
In the Nightmare scene, there are many notes scattered around, left by ~~iterations~~, which might guide you when you're lost or help you piece together a story from the past. To view a note, you need to approach and pick it up (see `/Operation Mode - Object Discarding and Picking Up/` for details). After picking up a note, an interface for viewing the note's content will automatically pop up. In this interface, the mouse will be automatically released, allowing you to view the content by dragging or scrolling the mouse wheel. To close the interface, you can click the `X` button at the top right corner of the screen or press the corresponding key on the keyboard: 
   - Press `[F]/[D-Pad Up]` to view the note
   - Press `[Enter]/[D-Pad Down]` to close the viewing interface  

***Note: You cannot control the character or weapons while viewing a note, but enemies can move normally. Therefore, ensure the environment is safe before viewing a note.**

### Options Menu
In Nightmare, you can summon the options menu in-game by pressing a key. When this happens, the mouse will be unlocked, and the player won't be able to move. You can perform operations here. Notably, for the "Game Settings" option, clicking it will bring up a new settings menu where you can edit options. After closing this settings menu, the changes will be automatically applied.You can perform actions through the following keys:
   - Press the `[Esc]` key to summon the options menu.
   - Press both `[Left Stick] & [Right Stick]` keys simultaneously to summon the options menu.  

***Note: Your changes will be stored and remembered when you launch the game next time.**

### Interactive UI Elements
In Nightmare, the interactive UI elements are divided into two types: buttons and checkboxes. Typically, buttons appear gray, and checkboxes display different colors according to their current state:
   - Gray: The checkbox is not selected/not enabled.
   - White: The checkbox is selected/enabled.

When the mouse pointer enters the range, the button/checkbox turns white and plays a prompt sound. Clicking a button will directly perform the corresponding action, while clicking a checkbox will toggle its state.

### Automatic Archiving System
The archiving system of Nightmare is automatic, fast, and requires no manual intervention. As long as you end the game through normal means, Nightmare will automatically save your progress upon exit, and will restore to the last save point when you start the game again next time. To delete a save, please click `Main Page - Clear Archive` or `Esc - Clear Archive`. After clearing the archive, it will automatically return to the main page. It is normal for there to be no response when clicking `Clear Archive` on the main page.

***Note: Under certain abnormal forced exits, the auto-save feature will not work.**

### Multiple Ending System
Nightmare features multiple endings, where any action or choice made in the game can affect the final outcome. Including the normal death scenario, Nightmare has 4 regular endings and 2 secret endings, and we look forward to players exploring these endings on their own.

<h2 id="section3">Implementation of Firearms</h2>

### Implementation of Firearms
All firearms in Nightmare are rendered separately from the scene, overlaying the depth view of the Player layer rendered by Gun Camera on top of Main Camera. Thus, all firearms in Nightmare can never penetrate through scene objects.

### Shooting Judgment
Nightmare uses two systems for judgment. For hit detection and damage deduction on enemies, it uses Physics.Raycast (ray detection) emitted from the gun muzzle. For the generation of bullet holes and interactive objects in the scene (such as oil drums, gas cylinders), and bullet holes, it uses real bullets based on Rigidbody physics, applying initial velocity to the bullets, and uses geometric node-based collision to determine impacts.

### Reloading of Firearms
For all rifles, pistols, and some sniper rifles with magazines, the bullets are instantly added when inserting the magazine. For all shotguns and some sniper rifles, the addition of bullets is according to the execution of the animation in the animation state machine, meaning the actual number of bullets naturally increases according to the animation of loading the bullets one by one.

###  Implementation of Zoom-in Effect Inside the Scope
All sniper rifles in Nightmare map the image to the front of the sniper scope in space using a special low FOV camera to achieve the zoom-in effect. The sniper scope will display different colors on the front glass when zooming in and out to simulate the real effect.

### Firearms Dropping and Picking Up
In Nightmare, Trigger collision boxes based on geometric nodes are mounted for interactive objects. Additionally, by adding a collision box with a special Tag as a sub-object to the player's camera, interactive objects can determine whether the player's view is facing the object. If the collision box under the player's camera collides with an interactive object, the hidden circular 2D image of the object is enabled. As the player and the object may be at different rotation angles, the UI image will maintain a vertical direction closest to the player and always face the player. It waits for the player to press the corresponding key and perform the corresponding operation. When discarding weapons, this project adopts a direct generation method, using physics-based MeshCollider and RigidBody to make the firearm object fall to the ground naturally due to gravity.

<h2 id="section4">Implementation of Enemies</h2>

### Overview of Enemies
There are currently only two types of enemies in Nightmare, Zombie and Mutant. Mutants are far stronger than zombies in terms of health, speed, and damage. Therefore, theoretically, zombies usually operate in groups under the leadership of mutants. Generally, enemies should patrol at walking speed when they have not discovered the player and run towards the player after spotting them. Of course, it's not uncommon for zombies or mutants to appear or even patrol alone.

### Enemy Detection of Players
Enemies will only detect players under certain conditions, as follows:
```swift
// Pseudocode
if (player enters the enemy's detection range)
{
    if (player is not crouching && player is moving) enemy detects player;
    if (player uses a gun without a silencer to shoot) enemy detects player;
    if (player has attacked enemy && enemy's health is less than 70%) enemy detects player;
    if (player reaches the enemy's absolute detection threshold) enemy detects player;
}else
{
    the enemy will not detect the player;
}

``` 
***Note: Different enemies have different detection ranges and absolute detection thresholds.**

### Enemy Movement
In Nightmare, the movement of all enemies is based on an optimized `A*` pathfinding algorithm using a `Navigation Mesh Agent`. During patrol, the enemies in the scene use the `Navigation Mesh Agent` to automatically move to their assigned patrol points. When an enemy is sufficiently close to a patrol point, the movement target automatically switches to the next patrol point, and this process repeats. When an enemy detects the player, the movement target switches to the player's real-time position. Once the player enters the enemy's attack range, which means the player is sufficiently close to the enemy, the enemy stops moving and initiates the attack logic.

### Enemy Attacks
Enemies will stop and initiate attack routines when they enter the attack range. Different types of enemies have different attack damages and cooldowns. For zombies, the enemy will have a shorter cooldown and lesser damage; for mutants, the enemy will have a longer cooldown and significantly more damage, and mutants will have explosive particle effects when attacking. Players in the enemy's attack range and when the enemy is on cooldown will perform an Idle animation.

### Enemy Damage Determination
In Nightmare, shooting at an enemy's head or body results in different damage. For determination, the enemy's body includes a `Mesh Collider` that is real-time baked at interval frames. Additionally, there is a `Sphere Collider` on the enemy's head, which is larger than the `Mesh Collider`. This setup allows the player's firearm raycast to detect the `Sphere Collider` before the `Mesh Collider`, prioritizing the detection of a headshot on the enemy.

<h2 id="section5">Scene Implementation</h2>

### Interactive Objects
#### Food
In Nightmare, the mechanism of picking up food is similar to that of firearms, judged by a trigger collision box based on geometry nodes, determining the collision with the player's arm. When the player's viewpoint is facing the food, it judges whether the corresponding key is pressed by the player. If the player presses the pick-up key, the food will self-destruct after calling the player's audio source to play the pick-up sound and the health regeneration coroutine. The player's coroutine will continue to play chewing sound effects after the food is destroyed and will continue for different chewing durations based on the data of the food picked up, restoring different amounts of health.

#### Notes
In Nightmare, all note scripts inherit from the same base class. This base class contains the content of all notes in the scene, while each individual note only contains the text ID. The viewing interface for the note is triggered when the collision box of the geometric node's Trigger detects a collision with the player's arm and the player presses the pickup key. This unlocks the cursor and displays the viewing view. The viewing interface of the note consists of a pinned `X` button and a Scroll View with a hidden scrollbar, allowing the player to naturally scroll or drag with the mouse to view the interface. During the note viewing process, all player movement is locked, and player input is constantly monitored, until the player presses `[Enter]/[D-Pad Down]` or clicks the `X` button, at which point the operation state is restored.

#### Oil Drums and Gas Cylinders
In the Nightmare scene, oil drums and gas cylinders can interact with bullets and the player. Oil drums and gas cylinders can be pushed by the player, and the explosion judgment of interactive objects is executed by physically fired bullets. Among them, when a bullet collides with an oil drum, the oil drum will instantly explode and release a large amount of damage within a wide range; when a bullet collides with a gas cylinder, the gas cylinder will leak gas from the top, and when the internal pressure reaches a threshold, it will explode, releasing a certain amount of damage within a relatively small range.  
  
***Note: The destruction of interactive objects in the scene does not affect the terrain.**

### Special Scenes
Some specific scenes in Nightmare will have certain effects on the player. For example, in water, players will be forced to crouch, cannot jump, and will play the sound of moving in water when moving. For all scenes entered by the player and at this time the height of the ceiling is less than the standing height of the player, the player will be forced to crouch. Even if the user releases `[Control]/[Left Shoulder]`, as long as the user does not press `[Control]/[Left Shoulder]` after leaving the scene, the player will automatically re-enter the standing state.

<h2 id="section6">Archive System</h2>

### System Overview
The save system of Nightmare adopts a simple and efficient method to save and load players' game progress. By serializing the player's position, health value, weapon status, and other information into JSON format and saving it to a file, the game's save and load functions are implemented. This not only ensures data persistence but also facilitates data management and migration.

### Creation of Save Files
To exit the game, users need to press the `Esc` key or switch window focus. Nightmare will capture and listen to these actions and automatically trigger the save process when these behaviors occur, theoretically covering all possible situations except for force quitting. This process involves collecting various information about the current game state, including but not limited to the player's position, character's orientation, current health value, weapons owned and their statuses (such as current ammo), enemy statuses, and other factors that may affect game progress. This information will be serialized and saved to a local file so that these states can be reloaded later.

### Loading Saves
When a player starts the game and a save file has already been saved, the game will read the previously saved save file and deserialize the information in the save, restoring the game state to the state at the time of the save. This means that the player's position, health value, weapon status, enemies' positions and statuses, and more will be restored, allowing players to continue the game from the exact point they left off.

### Clearing Saves
Players may wish to restart the game or need to clear existing saves in some cases. For this, we provide a feature to clear saves, allowing players to delete the current save file. You can find it in `Main Menu - Clear Saves` or `Esc - Clear Saves`. Nightmare will first ensure that the save file exists before performing the operation; otherwise, it will not proceed.

***Note: Clearing saves through `Esc - Clear Saves` will automatically return to the main menu to prevent the save from being automatically saved again.**

<h2 id="section7">Material Usage</h2>
The Nighmare has utilized a certain amount of open-source materials, covering most of the modeling, animation, and a very small number of prefabs. We express our admiration for the selfless open-source spirit of these material creators. The project has utilized the following materials:

   - Low Poly FPS Pack
   - Flooded Grounds Map Model
   - Mixamo Enemy models and animations
   - Zombie Character Sounds
   - Legacy Particle Pack Effects