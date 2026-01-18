using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient clientChat = new OllamaApiClient(new Uri("http://localhost:11434"), "gpt-oss:20b");

var historicoChat = new List<ChatMessage>();

Console.WriteLine("Digite 'sair' para encerrar");
Console.WriteLine();

while (true)
{
    Console.Write(">> ");
    var entrada = Console.ReadLine();
    
    if (entrada?.ToLower() == "sair")
        break;
    
    if (string.IsNullOrWhiteSpace(entrada))
        continue;
    
    historicoChat.Add(new ChatMessage(ChatRole.User, entrada));

    Console.WriteLine("");
    var resposta = string.Empty;

    await foreach (var atualizacao in clientChat.GetStreamingResponseAsync(historicoChat))
    {
        Console.Write(atualizacao.Text);
        resposta += atualizacao.Text;
    }
    
    historicoChat.Add(new ChatMessage(ChatRole.Assistant, resposta));
    Console.WriteLine("\n");
}