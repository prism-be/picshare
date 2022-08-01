// -----------------------------------------------------------------------
//  <copyright file = "JwtConfigurationFake.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Security;

namespace Prism.Picshare.Tests.Fakes;

public static class JwtConfigurationFake
{
    public static JwtConfiguration JwtConfiguration => new()
    {
        PrivateKey =
            "MIICXAIBAAKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQABAoGAJdEcGV2o9kzoC+frRC81k3yw1/Y32kZY+JmNZnmatU8UJHNaol7lHnA+PQutc+7JzXEVCP8A+AKC1D59pHs6gql24dCD1Ca9FfS5qTd4SIawUYqcL0OPSVkJPhL+/wVJVOLQG7ALiOhmyNZADSKCj/fKb3bQZH57Sj5osPqGZ4ECQQDwXyBhYwKXZTP4uYTpWX5or/NGF9t1vNdo5tzkspvOFmW81bbGoVg8wzshVmT9cLKlVFHMls1pDWqhCboGHevhAkEAx8x+MGb4haCwQVMnLmthCQmnB7n5uADv2KoVA0zXc7k/reRVgW7aSj4bKNnBrHqdhG8ELDCc02leSESe1J59DQJAevP9yTLvGWgADKNA9Gf9vCj8ZIdBj9kXyqYEqcse3W0hf1VGWBYh33rx3RynLeieyOj3qpIc4jalq1ghWo2loQJBALtMo5tCXIYAjlqe1iM4/H1ZdCDVIhlxn2awgwRV+7/7kIu2esXconxo3lMcV+gWBiZJYFMAu3Og2obK9U6CyN0CQDyfKjEQ4mveAMeYlnDTJmc2CaOVQ2je8SJd5dCwUZbF3nZPgWeuJ/R8jzQy+qxN3ICxRa+D1+ATNm0rmfnb25k=",
        PublicKey =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQAB"
    };
}