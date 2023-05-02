# GE2-Assignment
Penguins:
We're Here to Commit Arson

# Models

Polar bear model made and animated in blender

Model was made using similar techique to this example video, getting a rough shape from extruding a single node with a skin modifer then using sculpting workspace to
add more detail. Colored using a shader and color pattele grid to paint on indivial vertexs. Added armature with automatic weight painting and then animated
https://www.youtube.com/watch?v=DAAwy_l4jw4&t=1188s&ab_channel=JoeyCarlino

![image](https://user-images.githubusercontent.com/56588308/235794012-6559d26b-c7c3-4822-aa4e-01ac41db0d6b.png)

- Fish is a import open source model from Free3D, but custom skin, armature and animation added to it
- Custom egg model that wiggles



# Voxel Prototype using gdscript:
Some experimenting by Peter as he knows gdscript better then c#

Can be found in voxel_game branch, most of the unique assets are in voxel_stuff folder in godot
Created voxel models of penguin and polar bear using MagicaVoxel, which is a super easy voxel modeling tool, imported models to blender to animate them:

Animations: Idle, walk_loop, run_loop, eat_loop, swim_loop, attack

![Recording 2023-05-02 at 23 03 16](https://user-images.githubusercontent.com/56588308/235795991-65c31fa5-9a09-4dc2-90c8-ea5a5feb31fc.gif) ![Recording 2023-05-02 at 23 06 15](https://user-images.githubusercontent.com/56588308/235796214-674f5a01-2d91-45c8-b7d1-b193572e526c.gif)


# Logic

Polar Bear:
- The polar bear's `wander()` function sets a new random target position for the bear to walk towards within a certain `wander_radius`.
- The `check_for_penguins()` function checks if there are any penguins within the `charge_radius` distance of the polar bear. If there are, it sets the polar bear's target position to the closest penguin's position and changes the state to `State.CHARGING`.
- In the `charge()` function, the polar bear moves towards the target position at a faster speed (doubled speed) than in the wandering state. If the polar bear gets close enough to the target penguin (`attack_radius`), it changes the state to `State.ATTACKING`. If the polar bear gets close to its target position, it changes the state back to `State.WANDERING`.
- In the `attack()` function, the polar bear attacks the closest penguin within its attack radius. If it successfully attacks a penguin, it removes the penguin from the game and sets a new random target position for the polar bear to wander towards.

Penguin:
- The penguin's `wander()` function sets a new random target position for the penguin to walk towards within a certain `wander_radius`.
- The `check_for_polar_bears()` function checks if there are any polar bears within the `wander_radius` distance of the penguin. If there are, it sets the penguin's target position to the closest polar bear's position and changes the state to `State.RUNNING`.
- In the `run()` function, the penguin moves towards the target position at a faster speed (1.5 times the wandering speed) than in the wandering state. If the penguin gets far enough away from the target position, it changes the state back to `State.WANDERING`. 
- The penguin uses the `lerp_angle()` function to smoothly rotate towards its target position, instead of instantly snapping to face it.

# Gif of voxel game:
![Recording 2023-05-02 at 23 13 53](https://user-images.githubusercontent.com/56588308/235797333-4870c8b6-e2a1-48e1-98ba-23551ba77984.gif)
