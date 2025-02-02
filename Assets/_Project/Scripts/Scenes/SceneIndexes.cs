namespace LindoNoxStudio.Scenes
{
    /// <summary>
    /// Contains every scene with the corresponding index
    /// </summary>
    public enum SceneIndexes : int
    {
        #if Client
        Init = 0,
        Menu = 1,
        NetworkLayer = 2,
        Game = 3,
        #elif Server
        Init = 0,
        NetworkLayer = 1,
        Game = 2,
        #endif
    }   
}
