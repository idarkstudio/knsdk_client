using FigNet.Core;
using UnityEngine;

public class DBOperations : MonoBehaviour
{
    [ContextMenu("signup")]
    public void SignUpTest() 
    {
        var fName = "bilal";
        var sName = "A";
        var email = "test@fignet.dev";

        SignUp<UserModel>(fName, sName, email, (resp) => {

            Debug.Log($"Result Arrived... {resp.FirstName} | {resp.LastName} | {resp.Email}");
        });

    }


    public static void SignUp<T>(string firstName, string lastName, string email, System.Action<T> response) where T : class
    {
        var data = BitBufferPool.GetInstance();
        data.Clear();   // clear it to be 100 sure

        // add 1st byte as operation ID
        data.AddByte((byte)DBOperationID.SignUp);

        data.AddString(firstName);
        data.AddString(lastName);
        data.AddString(email);

        var requestData = data.ToArray();

        DBAccessModule.Send<T>(requestData.Array, response);
    }
}
