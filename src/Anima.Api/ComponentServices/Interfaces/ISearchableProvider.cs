using System;
using System.Threading.Tasks;
using Anima.Core.Dtos.Shared;


namespace Anima.Api.ComponentServices.Interfaces;

public interface ISearchableProvider
{
    Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId);
}