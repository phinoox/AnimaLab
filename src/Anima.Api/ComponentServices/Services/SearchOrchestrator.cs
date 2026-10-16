using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Core.Dtos.Shared;

namespace Anima.Api.ComponentServices.Services;

public class SearchOrchestrator
{
    private readonly IEnumerable<ISearchableProvider> _providers;
    private readonly ILogger<SearchOrchestrator> _logger;

    public SearchOrchestrator(IEnumerable<ISearchableProvider> providers, ILogger<SearchOrchestrator> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task<ListResponseDto<SearchHitDto>> GlobalSearchAsync(SearchRequestDto request)
    {
        try
        {
            var allHits = new List<SearchHitDto>();

            // 1. Filter providers by type if a specific type is requested
            var activeProviders = string.IsNullOrWhiteSpace(request.Type)
                ? _providers
                : _providers.Where(p => p.SearchType.Equals(request.Type, StringComparison.OrdinalIgnoreCase));

            // 2. Gather results from selected providers in parallel
            var tasks = activeProviders.Select(p => p.GetMatchesAsync(request.Query, request.ProjectId));
            var results = await Task.WhenAll(tasks);

            foreach (var hitList in results)
            {
                allHits.AddRange(hitList);
            }

            int totalCount = allHits.Count;
            var pagedResults = allHits
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
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