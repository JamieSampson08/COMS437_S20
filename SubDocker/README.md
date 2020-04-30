# SpaceDocker (WIP)

### **TODO/BUGS**:

- Picking up a torpedoPack doesn't add a torpedo object to the ship's list
- Target's Momentum's won't change
- Torpedo Aiming & Shooting
- When calling "AddTorepdo()", it says the physicsObject is null

**Note**
Thou these bugs do exist, surrounding code is implementd:
- torepdoPack's model exists and randomly populates the scene. Picking one up increases the number of torpedos you have "visual count wise" along with a notfication saying you have picked one up. 
- Storing and handling collisions for torpedos is programmed. The class exists with some of the functions completed. 
- Target's momentum code is copied and pasted from the RubberDUcks code to enable floating like movement, it just for some reason doesn't work for the target

### Tools:

- Monogame
- Bepuphysics v1
- Visual Studio 2019
- Blender

### Story:
You're traveling into the depths of the water to escape the clutches of the Rubber Duck society that has taken power over humans. Equipped only with fuel and torpedos, you venture out in hopes of locating the rare turtle that will enable you to take back your world.

### Goal:
Navigate through deep waters avoiding or destroying the Rubber Ducks who are chasing after you.

Locate the rare turtle before you lose all your fuel and/or health. Claiming this prize will make you the ruler of all Rubber Ducks!

### Messages:

- Torpedo Pack Picked Up
- Fuel Packs Picked Up
- No Torpedos Left
- Shield Active
- Fire Mode Active
- Health
- Fuel Level
- Torpedo Inventory
- Current Ship Position
- Target Position
- Success/Fail Game

### Controls:

*Note*: controls listed as keyboard/game controller

**General**

- Yaw: Q key/W key (rotate left, rotate right)
- Pitch: Left/Right (-, +)
- Roll: Down/Up (backward, forward)
- Thrust: T key / Y Button
- Restart: R key
- Show Target Position and Game Commands : H key
- Hid Target Position and Game Commands : B key

**Shield**

- Enable Shield : S key
- Disable Shield : D key

**Fire Mode**

- Enter Fire Mode: F key / Button

- Exit Fire Mode : G key / B button
- Aim Left : left arrow / left DPad
- Aim Right : right arrow / right DPad
- Fire Torpedo : spacebar / B button

*Notes:* 

- After firing a torpedo, game automatically exits fire mode

### Features:

- Turtle (Target) Moves
- Shield Capabilities
- Fuel & Torpedo Packs
