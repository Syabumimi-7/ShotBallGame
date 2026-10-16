using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteScore : MonoBehaviour
{
    private string inputHistory = "";
    private const string TARGET_COMMAND = "datadelete";

    void Update()
    {
        // 1. そのフレームで入力された文字を取得
        string input = Input.inputString;

        if (!string.IsNullOrEmpty(input))
        {
            // 2. 履歴に追加
            inputHistory += input;

            // 履歴が長くなりすぎないように制限（ターゲットの文字数分あればOK）
            if (inputHistory.Length > TARGET_COMMAND.Length)
            {
                inputHistory = inputHistory.Substring(inputHistory.Length - TARGET_COMMAND.Length);
            }

            // 3. 文字列が一致するか判定
            if (inputHistory == TARGET_COMMAND)
            {
                Delete();
                inputHistory = ""; // 実行後はリセット
            }
        }
    }

    void Delete()
    {
        PlayerPrefs.DeleteAll();
    }
}
