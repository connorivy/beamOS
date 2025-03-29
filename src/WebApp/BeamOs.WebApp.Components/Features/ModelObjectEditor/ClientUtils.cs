namespace BeamOs.WebApp.Components.Features.ModelObjectEditor;

public static class ClientUtils
{
    public static int GenerateTempId()
    {
        unchecked
        {
            int tempId = (int)DateTime.UtcNow.Ticks;
            return tempId < 0 ? tempId : -1 * tempId;
        }
    }
}
