using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GameInitializer : MonoBehaviour
{
    private CancellationTokenSource cts;

    async void Start()
    {
        cts = new CancellationTokenSource();
        try
        {
            await InitAsync(cts.Token);
            Debug.Log("All resources loaded successfully!");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Operation canceled");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception: {ex.Message}");
        }
    }

    async UniTask InitAsync(CancellationToken cancellationToken)
    {
        //var resource = new List<UniTaskCompletionSource<string>>();
        var resource = new List<UniTask<string>>();
        foreach (var resourceUrl in GetResourceUrl())
        {
            resource.Add(LoadResourceAsync(resourceUrl, cancellationToken));
        }

        var results = await UniTask.WhenAll(resource);
        foreach (var result in results)
        {
            Debug.Log($"Loaded: {result}");
        }
    }

    IEnumerable<string> GetResourceUrl()
    {
        yield return "1.png";
        yield return "2.png";
        yield return "3.png";
    }

    async UniTask<string> LoadResourceAsync(string resourceUrl, CancellationToken cancellationToken)
    {
        try
        {
            // 模拟异步加载（替换为实际资源加载逻辑）
            await UniTask.Delay(2000, cancellationToken: cancellationToken);

            Debug.Log($"Successfully loaded: {resourceUrl}");
            return resourceUrl;
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"Loading canceled: {resourceUrl}");
            throw;
        }


        // cancellationToken.Register(() =>
        // {
        //     if (!tcs.Task.IsCompleted)
        //     {
        //         tcs.SetCanceled();
        //     }
        // });

        //return await tcs.Task;
    }

    void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}