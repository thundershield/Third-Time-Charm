public static class Rooms
{
    public const char Air = ' ';
    public const char Solid = '#';
    public const char Spawn = '[';
    public const char Exit = ']';
    public const int RoomWidth = 10;
    public const int RoomHeight = 8;

    public static readonly string[] AllOpenSpawn = {
        "####  ####" +
        "#        #" +
        " ^       #" +
        "##      ##" +
        "#   [    #" +
        "#  ####  #" +
        "          " +
        "####  ####"
    };
        
    public static readonly string[] AllOpenExit = {
        "####  ####" +
        "##      ##" +
        "#        #" +
        "#        #" +
        "#        #" +
        "#        #" +
        "      ]   " +
        "####  ####"
    };

    public static readonly string[] LeftRightOpen = {
        "##########" +
        "#        #" +
        "#        #" +
        "#        #" +
        "#        #" +
        "#        #" +
        "          " +
        "##########"
    };
        
    public static readonly string[] AllOpen = {
        "####  ####" +
        "##      ##" +
        "#        #" +
        "# #####  #" +
        "#        #" +
        "##      ##" +
        "          " +
        "####  ####",
        "####  ####" +
        "#        #" +
        "# #    # #" +
        "# #    # #" +
        "# ##  ## #" +
        "#        #" +
        "  #    #  " +
        "####  ####",
        "####  ####" +
        "#        #" +
        "#       ##" +
        "#     ####" +
        "##       #" +
        "###      #" +
        "      #   " +
        "####  ####",
        "####  ####" +
        "#   #    #" +
        "#     ####" +
        "##   #####" +
        "###      #" +
        "#      ###" +
        "   #      " +
        "####  ####"
    };

    public static readonly string[] Optional = {
        "####  ####" +
        "#        #" +
        "###      #" +
        "##     ###" +
        "#     ####" +
        "###      #" +
        "#####    #" +
        "##########",
        "##########" +
        "###    ###" +
        "##      ##" +
        "##      ##" +
        "#        #" +
        "#        #" +
        "#  #  #   " +
        "##########",
        "##########" +
        "###    ###" +
        "##      ##" +
        "##      ##" +
        "#        #" +
        "#        #" +
        "   #  #  #" +
        "##########",
        "##########" +
        "#        #" +
        "###      #" +
        "####     #" +
        "###      #" +
        "####    ##" +
        "#      ###" +
        "####  ####"
    };
}