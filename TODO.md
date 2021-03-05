# Maths For Games TODO List

## LC ECS
*Lewis Comstive's Entity Component System*
 - [ ] Write unit tests
 - [ ] Create documentation
 - [ ] Update `README.md` for usage instructions and description

## LC Utilities
*Lewis Comstive's Utility Classes*
 - [x] Write unit tests
 	- [ ] Write MORE unit tests
 - [ ] Add `README.md` for usage instructions, description and examples
 - [ ] Create documentation

## Game
An application to showcase the libraries' capabilities

##### General
 - [ ] Use a proper coordinate system, rather than just using screenspace
 - [ ] Somehow use `LC Utilities`' matrix classes (*as per class assignment*)
	- Potentially for the 2D camera component's orthographic projection?

##### Components
 - [x] Transform
 - [ ] Camera
 - [ ] Coloured rectangle
 - [ ] Rigidbody
 - [ ] Collider
 - [ ] Sprite

##### Systems
 - [x] Basic coloured rectangle renderer
 - [ ] Sprite renderer
 - [ ] Text renderer (*Using coordinate system, not screen-space!*)
 - [ ] Player input handler
 - [ ] Basic 2D physics
	- At this point the application may be using exclusively 2D rectangles,
		so AABB collision detection will suffice.

##### Game mechanics
 - [ ] Come up with something