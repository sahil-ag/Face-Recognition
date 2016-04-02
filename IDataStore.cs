using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack_in_the_north_hand_mouse
{
    interface IDataStore
    {
        String SaveFace(String username, Byte[] faceBlob);
        List<face> CallFaces(String username);
        bool IsUsernameValid(String username);
        String SaveAdmin(String username, String password);
        bool DeleteUser(String username);
        int GetUserId(String username);
        int GenerateUserId();
        String GetUsername(int userId);
        List<String> GetAllUsernames();
    }
}
