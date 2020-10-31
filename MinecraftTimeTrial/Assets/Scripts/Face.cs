using UnityEngine;

public class Face
{
    public FaceType faceType;
    public int x;
    public int y;
    public Vector2[] uvs;

    public Face(FaceType faceType, int x, int y)
    {
        this.faceType = faceType;
        this.x = x;
        this.y = y;

        uvs = new Vector2[]
        {
            new Vector2(x * TextureData.NormalizedBlockTextureSize, y * TextureData.NormalizedBlockTextureSize),
            new Vector2(x * TextureData.NormalizedBlockTextureSize, (y + 1) * TextureData.NormalizedBlockTextureSize),
            new Vector2((x + 1) * TextureData.NormalizedBlockTextureSize, y * TextureData.NormalizedBlockTextureSize),
            new Vector2((x + 1) * TextureData.NormalizedBlockTextureSize, (y + 1) * TextureData.NormalizedBlockTextureSize)
        };
    }
}
