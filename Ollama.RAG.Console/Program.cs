using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.KernelMemory.Configuration;

Console.WriteLine("\nConsultando Manual de Políticas da Empresa (RAG)\n");

var config = new OllamaConfig
{
    Endpoint = "http://localhost:11434",
    TextModel = new OllamaModelConfig("deepseek-r1:8b", 131072),
    EmbeddingModel = new OllamaModelConfig("nomic-embed-text", 768)
};

var memoryBuilder = new KernelMemoryBuilder()
    .WithOllamaTextGeneration(config)
    .WithOllamaTextEmbeddingGeneration(config)
    .WithCustomTextPartitioningOptions(new TextPartitioningOptions
    {
        MaxTokensPerParagraph = 256,
        OverlappingTokens = 30
    });
    
var kernelMemory = memoryBuilder.Build();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Iniciando ingestão de documentos...");

try
{
    await kernelMemory.ImportDocumentAsync(filePath: "politica.txt", documentId: "POL001");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Documentos ingeridos com sucesso!\n");
    Console.ResetColor();
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Erro ao ingerir documentos:");
    Console.WriteLine(e.Message);
    Console.ResetColor();
}

Console.WriteLine("Modelo pronto para responder as perguntas");

while (true)
{
    Console.Write(">> ");
    var entrada = Console.ReadLine();

    if (entrada?.ToLower() == "sair")
        break;

    if (string.IsNullOrWhiteSpace(entrada))
        continue;

    var promptSeguro = $"Responda a pergunta com base nos documentos fornecidos.\n\nPergunta: {entrada}";
    var resposta = await kernelMemory.AskAsync(promptSeguro);

    Console.WriteLine(resposta.Result);

    if (resposta.RelevantSources.Count == 0)
        Console.WriteLine("Nenhuma fonte relevante encontrada.");
    else
        foreach (var source in resposta.RelevantSources)
            Console.WriteLine($"- Arquivo: {source.SourceName}, Trecho: {source.Partitions.FirstOrDefault()}");
    
    Console.WriteLine();
}