using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StockSentimentAnalyzer.Domain.Models;

namespace StockSentimentAnalyzer.Api.Hubs;

/// <summary>
/// SignalR hub for real-time sentiment updates
/// </summary>
[Authorize]
public class SentimentHub : Hub
{
    /// <summary>
    /// Sends a sentiment analysis update to all connected clients
    /// </summary>
    /// <param name="analysis">The sentiment analysis result</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task SendSentimentUpdate(SentimentAnalysis analysis)
    {
        await Clients.All.SendAsync("ReceiveSentimentUpdate", analysis);
    }

    /// <summary>
    /// Subscribes a client to updates for specific stock symbols
    /// </summary>
    /// <param name="symbols">List of stock symbols to subscribe to</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task SubscribeToSymbols(IEnumerable<string> symbols)
    {
        var existingGroups = Context.Items["SubscribedSymbols"] as HashSet<string> ?? new HashSet<string>();
        
        foreach (var symbol in symbols)
        {
            if (!existingGroups.Contains(symbol))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
                existingGroups.Add(symbol);
            }
        }
        
        Context.Items["SubscribedSymbols"] = existingGroups;
    }

    /// <summary>
    /// Unsubscribes a client from updates for specific stock symbols
    /// </summary>
    /// <param name="symbols">List of stock symbols to unsubscribe from</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task UnsubscribeFromSymbols(IEnumerable<string> symbols)
    {
        var existingGroups = Context.Items["SubscribedSymbols"] as HashSet<string> ?? new HashSet<string>();
        
        foreach (var symbol in symbols)
        {
            if (existingGroups.Contains(symbol))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
                existingGroups.Remove(symbol);
            }
        }
        
        Context.Items["SubscribedSymbols"] = existingGroups;
    }

    /// <summary>
    /// Sends a sentiment update for a specific stock symbol
    /// </summary>
    /// <param name="symbol">The stock symbol</param>
    /// <param name="analysis">The sentiment analysis result</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task SendSymbolUpdate(string symbol, SentimentAnalysis analysis)
    {
        await Clients.Group(symbol).SendAsync("ReceiveSymbolUpdate", symbol, analysis);
    }
}
