namespace FistOfTheFree.Guns
{
    // Defines the shooting mechanism for a raycasr gun.
    public enum ShootType
    {
    
        // Allows for a raycast to happen from the center of screen and a trail/bullet will be shot towards the hit point from the camera (where the player is looking).
        FromCamera,
        // Allows for shooting to happen from the barrel of the gun instead of from the center of the screen.
        FromGun
    }

}
