public class Block
{
    public BlockType blockType;
    public Face top;
    public Face side;
    public Face bottom;    

    public Block(BlockType blockType, FaceType faceType)
    {
        this.blockType = blockType;

        Face face = TextureData.faces[faceType];
        top = face;
        side = face;
        bottom = face;
    }

    public Block(BlockType blockType, FaceType topType, FaceType sideType, FaceType bottomType)
    {
        this.blockType = blockType;

        top = TextureData.faces[topType];
        side = TextureData.faces[sideType];
        bottom = TextureData.faces[bottomType];
    }

    public Face GetFace(int faceIndex)
    {
        if (faceIndex == 2)
        {
            return top;
        }
        else if (faceIndex == 3)
        {
            return bottom;
        }
        else
        {
            return side;
        }
    }
}
