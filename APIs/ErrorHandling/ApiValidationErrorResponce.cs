﻿namespace APIs.ErrorHandling
{
    public class ApiValidationErrorResponce : ApiResponce
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponce() : base(400)
        {
            Errors = new List<string>();
        }
    }
}
