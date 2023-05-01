# GE2-Assignment Budget Club Penguin

All jokes aside, this project is a life simulation of a group of penguins in a glacier surrounded by water. The penguins will have to survive in this glacier, whilst having one of their natural predators also there, the polar bear. The bear will try to track a penguin, and once he does he will then follow him and eventually kill him. The penguins will have to develop ways to evade their predator while also keeping their population afloat through procreation. This project took inspiration from the well-known Game of Life by John Conway.

________________________________________________________________________________________________________________________________________________________________

## Penguins:

The penguins are kinematic bodies that will spawn in the glacier. Once they spawn, they will walk in a given random location as long as that location does not intersect with another penguin or when it senses the edge of the glacier. This is their initial state however, when the polar bear enters their detection range, it will enter into a fear state, which will make them run in the opposite direction of the polar bear. When two penguins meet, they will lay an egg. The egg will take approximately 3-4 seconds to hatch. While it's an egg the predator will not target it. The penguins will need to periodically feed themselves by eating the fish that comes ashore by the rising sea water.

________________________________________________________________________________________________________________________________________________________________

## Polar Bear:

The polar bear much like the penguin is also a kinematic body. It will start in a wandering state, which essentially means that it will walk in a random location until it senses a penguin. When it does it will run towards the penguin, it will catch up to it and eat it. The polar will spawn in with a full hunger bar, making it slow. Once the bar is depleted, the bear will enter a frenzy state where its movement speed will increase and will actively try to find penguins to satiate his hunger.

________________________________________________________________________________________________________________________________________________________________

## Water:

For the water, we took inspiration from the following website "https://catlikecoding.com/unity/tutorials/flow/waves/", which is a tutorial for creating a simulation of waves on Unity. We took some of the concepts in this post and re-created them in GODOT to try and accurately depict the cold waters of the Arctic.

________________________________________________________________________________________________________________________________________________________________
