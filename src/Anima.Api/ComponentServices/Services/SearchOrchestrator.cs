using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Core.Dtos.Shared;

namespace Anima.Api.ComponentServices.Services;

internal class SearchOrchestrator
{
    private readonly IEnumerable<ISearchableProvider> _providers;
    private readonly ILogger<SearchOrchestrator> _logger;

    public SearchOrchestrator(IEnumerable<ISearchableProvider> providers, ILogger<SearchOrchestrator> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task<ListResponseDto<SearchHitDto>> GlobalSearchAsync(string query, Guid? projectId, int page, int pageSize)
    {
        try
        {
            var allHits = new List<SearchHitDto>();

            var tasks = _providers.Select(p => p.GetMatchesAsync(query, projectId));
            var results = await Task.WhenAll(tasks);

            foreach (var hitList in results)
            {
                allHits.AddRange(hitList);
            }

            int totalCount = allHits.Count;
            var pagedResults = allHits
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ListResponseDto<SearchHitDto>
            {
                Items = pagedResults,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during global search orchestration.");
            throw;
        }
    }
}