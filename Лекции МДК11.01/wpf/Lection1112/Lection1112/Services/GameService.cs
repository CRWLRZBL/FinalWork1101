using Lection1112.Models;
using Lection1112.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection1112.Services
{
    public class GameService
    {
        private readonly GameContext _context = new();

        public async Task<List<Game>> GetGamesAsync()
            => await _context.Games
            .Include(g => g.Category)
            .ToListAsync();

        public async Task<List<Category>> GetCategoriesAsync() 
            => await _context.Categories
            .ToListAsync();

        public async Task DeleteGameAsync(Game game)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        public async Task AddGameAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGameAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
    }
}
