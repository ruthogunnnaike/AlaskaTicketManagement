﻿namespace AlaskaTicketManagement.Contracts
{
    public class VenueRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
