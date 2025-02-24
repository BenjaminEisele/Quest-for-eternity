using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConecctionID;
    public ulong PlayerSteamID;
    private bool AvatarRecieved;
    public TextMeshProUGUI PlayerNameText;
    public GameObject PlayerReady;
    public RawImage PlayerIcon;
    public bool Ready;
    public GameObject ServerReadySpawnPoint;
    public GameObject ClientReadySpawnPoint;
    private bool isFirstTime = true;
    [SerializeField]
    BeerMugTween tweeningObject;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void SetPlayerValues(bool server)
    {
        PlayerNameText.text = PlayerName;
        ChangeReadyStatus(server);
        if (!AvatarRecieved) { GetPlayerIcon(); }
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (ImageID == -1) { return; }
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarRecieved = true;
        return texture;
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }

    public void ChangeReadyStatus(bool server)
    {
        if (Ready)
        {
            PlayerReady.SetActive(true);
            tweeningObject.ReadyTween(server);
            isFirstTime = false;
        }

        else
        {
            //PlayerReady.SetActive(false);
            if (!isFirstTime)
            {
                tweeningObject.ResetTween(server);
            }
        }
    }

}
