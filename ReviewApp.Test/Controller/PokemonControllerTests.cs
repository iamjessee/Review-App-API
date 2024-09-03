using AutoMapper;
using AutoMapper.Execution;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewApp.Controllers;
using ReviewApp.Dto;
using ReviewApp.Interfaces;
using ReviewApp.Models;
using System.Xml.Serialization;

namespace ReviewApp.Test.Controller
{
    public class PokemonControllerTests
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        public PokemonControllerTests()
        {
            _pokemonRepository = A.Fake<IPokemonRepository>();
            _reviewRepository = A.Fake<IReviewRepository>();
            _mapper = A.Fake<IMapper>();
        }

        [Fact]
        public void PokemonController_GetPokemons_ReturnsOk()
        {
            // arrange
            var pokemons = A.Fake<ICollection<Pokemon>>();
            var pokemonList = A.Fake<List<PokemonDto>>();

            A.CallTo(() => _pokemonRepository.GetPokemons()).Returns(pokemons);
            A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonList);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemons() as OkObjectResult;

            //assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public void PokemonController_GetPokemon_ReturnOK()
        {
            // arrange
            var pokemons = A.Fake<ICollection<PokemonDto>>();
            var pokemonList = A.Fake<List<PokemonDto>>();

            A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonList);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemons() as OkObjectResult;

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeEquivalentTo(pokemonList);
        }

        [Fact]
        public void PokemonController_GetPokemon_Returns500OnException()
        {
            // arrange
            A.CallTo(() => _pokemonRepository.GetPokemons()).Throws(new Exception());

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.GetPokemons() as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void PokemonController_CreatePoke_ReturnOK()
        {
            // arrange
            int ownerId = 1;
            int catId = 2;
            var pokemonMap = A.Fake<Pokemon>();
            var pokemon = A.Fake<Pokemon>();
            var pokemonCreate = A.Fake<PokemonDto>();
            var pokemons = A.Fake<ICollection<PokemonDto>>();
            var pokemonList = A.Fake<IList<PokemonDto>>();

            A.CallTo(() => _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Returns(pokemon);
            A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemon);
            A.CallTo(() => _pokemonRepository.CreatePokemon(ownerId, catId, pokemon)).Returns(true);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.CreatePokemon(ownerId, catId, pokemonCreate);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void PokemonController_CreatePoke_ReturnNoContent()
        {
            // arrange


            //act


            //assert
        }
    }
}
