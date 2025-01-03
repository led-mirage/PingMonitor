// © 2024 led-mirage. All rights reserved.

using System.Net.NetworkInformation;

public static class PingHelper
{
    public static async Task<PingReply> SendPingAsync(
        string host, int timeout, CancellationToken cancellationToken, AddressFamilyPreference preference = AddressFamilyPreference.Auto)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var ping = new Ping();
        string ipaddress = DnsHelper.GetHostAddress(host, preference) ?? throw new PingException("Failed to resolve host address");
        Task<PingReply> pingTask = ping.SendPingAsync(ipaddress, timeout);

        try
        {
            // SendPingAsyncメソッドはキャンセル非対応なので、
            // Task.WhenAnyとTask.Delayとを組み合わせてキャンセルを実現する
            Task completedTask = await Task.WhenAny(pingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            
            if (completedTask == pingTask)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await pingTask;
            }
            else
            {
                throw new OperationCanceledException("Ping was canceled", cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 処理がキャンセルされた場合にここにくる
            // そのまま再スローする
            throw;
        }
        catch (Exception ex)
        {
            // 予期しない例外が発生した場合は、PingException として再スローする
            throw new PingException("Ping operation failed", ex);
        }
    }

    public static async Task<PingReply> SendPingAsync(
        string host, CancellationToken cancellationToken, AddressFamilyPreference preference = AddressFamilyPreference.Auto)
    {
        // タイムアウトのデフォルト値は 5000 ミリ秒とする
        return await SendPingAsync(host, 5000, cancellationToken, preference);
    }

    public static async Task<PingReply> SendPingWithRetryAsync(
        string host, int timeout, int retryCount, CancellationToken cancellationToken, AddressFamilyPreference preference = AddressFamilyPreference.Auto)
    {
        PingReply? pingReply = null;

        for (int attempt = 0; attempt <= retryCount; attempt++)
        {
            try
            {
                pingReply = await SendPingAsync(host, timeout, cancellationToken, preference);
                
                if (pingReply.Status == IPStatus.TimedOut)
                {
                    continue;
                }

                if (pingReply.Status == IPStatus.Success)
                {
                    return pingReply;
                }
            }
            catch (OperationCanceledException)
            {
                // 処理がキャンセルされた場合にここにくる
                // そのまま再スローする
                throw;
            }
            catch (Exception ex)
            {
                // 予期しない例外が発生した場合は、PingException として再スローする
                throw new PingException("Ping operation failed", ex);
            }
        }

        return pingReply ?? throw new PingException("Ping operation failed");
    }

    public static async Task<PingReply> SendPingWithRetryAsync(
        string host, int retryCount, CancellationToken cancellationToken, AddressFamilyPreference preference = AddressFamilyPreference.Auto)
    {
        return await SendPingWithRetryAsync(host, 5000, retryCount, cancellationToken);
    }
 }
