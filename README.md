This repo contains my implementation for object manipulation on Unity 2020.3.42.

Build instructions:
1) Clone this repo and open it using Unity 2020.3.42 LTS.
2) Once open, go to Assets/ and open the SampleScene. The scene should already have scripts/game objects set up.
   - If it does not for whatever reason, the set up should be as follows:
     MeshCollection.cs to MeshCollection and add the following scripts
       Element 0 - Cubert
       Element 1 - Capsule
       Element 2 - Cylinder
     ApplyTransforms.cs to Cubert, Capsule, Cylinder. The following fields should be set up
     ScaleFactor to 0.01, Translate Factor to 0.01, Angular Velocity to 2.
     
3) Use the LEFT or RIGHT arrow keys to cycle between objects. Feel free to add more objects and attach the ApplyTransforms.cs script to it. 
   Note that they should all have a scale of (1, 1, 1) otherwise there'll be funky behavior.
