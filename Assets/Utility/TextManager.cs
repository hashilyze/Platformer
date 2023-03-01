namespace Utility
{
    public static class TextManager
    {
        public static string MakeNullComponentReferenceMessage (System.Type type)
        {
            return $"Could not find '{type.Name}' type component; Attach the component to gameobject";
        }
    }
}