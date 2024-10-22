using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPIEmployesSalary.Bussiness;
using WebAPIEmployesSalary.Model;
using WebAPIEmployesSalary.Services;
using WebAPIEmployesSalary.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace WebAPIEmployesSalary.Controllers
{
    //[Route("[controller]")]
    [ApiController]    
    [Route("api/[controller]")]
    public class EmployController : ControllerBase
    {
        private readonly EmployeeApiService _employeeApiService;
        private readonly ILogger<EmployController> _logger;
        private readonly IEmployController _employService;
        private readonly ILogger<WeatherForecastController> _logger2;
        private readonly IConfiguration _configuration;

        readonly HttpClient client = new HttpClient();

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public EmployController(EmployeeApiService employeeApiService, ILogger<EmployController> logger, IEmployController employService, ILogger<WeatherForecastController> logger2, IConfiguration configuration)
        {
            _employeeApiService = employeeApiService;
            _logger = logger;
            _employService = employService;
            _logger2 = logger2;
            _configuration = configuration;
        }

        //[HttpPost("calcular-salario-anual")]
        //public ActionResult<decimal> CalcularSalarioAnual([FromBody] Employ empleado)
        //{
        //    ICalculoSalarioStrategy estrategia;

        //    // Aquí decides qué estrategia usar
        //    if (empleado.SalarioMensual > 0)
        //    {
        //        estrategia = new CalculoSalarioMensual();
        //    }
        //    else if (empleado.SalarioPorHora > 0)
        //    {
        //        estrategia = new CalculoSalarioPorHora();
        //    }
        //    else
        //    {
        //        return BadRequest("Datos de salario inválidos.");
        //    }

        //    var calculadoraSalario = new CalculadoraSalario(estrategia);
        //    var salarioAnual = calculadoraSalario.CalcularSalarioAnual(empleado);

        //    return Ok(salarioAnual);
        //}
        //public async Task<IActionResult> Get(int? id)
       

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            ICalculatorSalaryStrategy estrategia;
            string urlComplement = string.Empty;
            string url = string.Empty;
            //List<EmployResponse> lstEmployees = new List<EmployResponse>();
            EmployResponse lstEmployees = new EmployResponse();

            //if (id != null)
            //     urlComplement = "/" + id;

            url = "http://dummy.restapiexample.com/api/v1/employees" + urlComplement;


            try
            {
                using (var client = new HttpClient())
                {
                    var urlBase = _configuration["URLApiEmployees"];
                    //var endpointUrl = new Uri($"http://dummy.restapiexample.com/api/v1/employees", UriKind.Absolute);
                    var endpointUrl = new Uri($""+urlBase, UriKind.Absolute);
                    var setting1 = _configuration["MySettings:Setting1"];

                    var response = await client.GetAsync(endpointUrl);
                    response.EnsureSuccessStatusCode();

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var lstEmployees2 =await JsonSerializer.DeserializeAsync<IEnumerable<EmployResponse>>(contentStream);
                    }
                    //var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v3/search/product");
                    //var lstEmployees3= await GetAsync<EmployResponse>(requestMessage);
                }
             }

              catch (HttpRequestException e)
              {
               Console.WriteLine("Message :{0} ", e.Message);
              }

            try
            {

                lstEmployees = await _employeeApiService.ObtenerDatosAsync<EmployResponse>(url);

                for (int i = 0; i < lstEmployees.Data.Count; i++)
                {
                    if (lstEmployees.Data[i].Salary > 0)
                    {
                        estrategia = new CalculatorAnnualSalary();
                        lstEmployees.Data[i].AnnualSalary = estrategia.OperationCalculatorAnnualSalary(lstEmployees.Data[i]);
                    }
                }
                //response.EnsureSuccessStatusCode();


                return Ok(lstEmployees);
            }
            catch (Exception ex)
            {
                // Loguear el error (opcional)
                // Devuelve un código de estado apropiado y un mensaje de error
                return StatusCode(500, new { Mensaje = ex.Message });
            }

            //var datos = await _employeeApiService.ObtenerDatosAsync<EmployAnnualSalary>("https://api.ejemplo.com/datos");
            //return Ok(datos);

        }


    }
}
