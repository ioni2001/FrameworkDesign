using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using ToDoMauiClient.Models;
using System.Text.Json;

namespace ToDoMauiClient.DataServices;

public class RestDataService : IRestDataService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseAddress;
    private readonly string _url;
    private readonly JsonSerializerOptions _jsonSerializeOptions;

    public RestDataService()
    {
        _httpClient = new HttpClient();
        _baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5257" : "https://localhost:7088";
        _url = $"{_baseAddress}/api";

        _jsonSerializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public async Task AddToDoAsync(ToDo toDo)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("----> No internet access...");
            return;
        }

        try
        {
            var jsonToDo = JsonSerializer.Serialize<ToDo>(toDo, _jsonSerializeOptions);
            var content = new StringContent(jsonToDo, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_url}/todo", content);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Successfully created ToDo");
            }
            else
            {
                Debug.WriteLine("----> Non Http 2xx response");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception is: {ex.Message}");
        }
    }

    public async Task DeleteToDoAsync(int id)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("----> No internet access...");
            return;
        }

        try
        {
            var response = await _httpClient.DeleteAsync($"{_url}/todo/{id}");

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Successfully deleted ToDo");
            }
            else
            {
                Debug.WriteLine("----> Non Http 2xx response");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception is: {ex.Message}");
        }
    }

    public async Task<List<ToDo>> GetAllToDosAsync()
    {
        var todos = new List<ToDo>();

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("----> No internet access...");
            return todos;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_url}/todo");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                todos = JsonSerializer.Deserialize<List<ToDo>>(content, _jsonSerializeOptions);
            }
            else
            {
                Debug.WriteLine("----> Non Http 2xx response");
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Exception is: {ex.Message}");
        }

        return todos;
    }

    public async Task UpdateToDoAsync(ToDo toDo)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("----> No internet access...");
            return;
        }

        try
        {
            var jsonToDo = JsonSerializer.Serialize<ToDo>(toDo, _jsonSerializeOptions);
            var content = new StringContent(jsonToDo, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_url}/todo/{toDo.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Successfully updated ToDo");
            }
            else
            {
                Debug.WriteLine("----> Non Http 2xx response");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception is: {ex.Message}");
        }
    }
}
