using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var config=new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var deploymentName=config["AzureOpenAI:DeploymentName"]?? 
throw new InvalidOperationException("Missing deployment Name");
var endpoint=config["AzureOpenAI:Endpoint"]?? 
throw new InvalidOperationException("Missing endpoint Name");
var apiKey=config["AzureOpenAI:ApiKey"]?? 
throw new InvalidOperationException("Missing ApiKey Name");
var builder=Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(deploymentName,endpoint,apiKey);
builder.Services.AddLogging(logging=>logging.AddConsole().SetMinimumLevel(LogLevel.Information));
var kernel=builder.Build();
var chat=kernel.GetRequiredService<IChatCompletionService>();
var history=new ChatHistory("You are an assistant for business app demos");
Console.WriteLine("Type messages.Type exit to quit");
while(true)
{
    Console.Write("User >");
    var input=Console.ReadLine();
    if(input==null || input.Equals("exit",StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
    history.AddUserMessage(input);
    var response=await chat.GetChatMessageContentAsync(history);
    var text=response.Content??string.Empty;
    history.AddMessage(response.Role,text);
    Console.WriteLine($"Assistant > {text}");

}

