namespace FistOfTheFree.Guns
{
    // Defines how spread is calculated.
    public enum BulletSpreadType
    {
   
        // No Spread is ever calculated. Will always shoot directly forward
 
        None,
        // Picks random values in each of X,Y,Z as defined in "ShootConfigScriptableObject.Spread"
        Simple,
        // Spread calculated based on a provided texture's greyscale value. 
        TextureBased
    }
}