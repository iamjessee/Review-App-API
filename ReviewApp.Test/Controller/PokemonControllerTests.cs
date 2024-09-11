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
        public void GetPokemons_ValidRequest_ReturnsOkResult()
        {
            // arrange
            var pokemons = A.Fake<ICollection<PokemonDto>>();
            var pokemonList = A.Fake<List<PokemonDto>>();

            A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonList);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemons();

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(pokemonList);
            result.As<OkObjectResult>().StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public void GetPokemons_ExceptionThrown_ReturnsInternalServerError()
        {
            // arrange
            A.CallTo(() => _pokemonRepository.GetPokemons()).Throws(new Exception());

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemons();

            // assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void GetPokemon_ValidId_ReturnsOkResult()
        {
            // arrange
            int pokeId = 1;
            var pokemon = A.Fake<Pokemon>();
            var pokemonDto = A.Fake<PokemonDto>();

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(true);
            A.CallTo(() => _pokemonRepository.GetPokemon(pokeId)).Returns(pokemon);
            A.CallTo(() => _mapper.Map<PokemonDto>(pokemon)).Returns(pokemonDto);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemon(pokeId);

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(pokemonDto);
            result.As<OkObjectResult>().StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public void GetPokemon_InvalidId_ReturnsNotFound()
        {
            // arrange
            int pokeId = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(false);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemon(pokeId);

            // assert
            result.Should().BeOfType<NotFoundResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public void GetPokemon_ExceptionThrown_ReturnsInternalServerError()
        {
            // arrange
            int pokeId = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(true);
            A.CallTo(() => _pokemonRepository.GetPokemon(pokeId)).Throws(new Exception());

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemon(pokeId);

            // assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void GetPokemonRating_ValidId_ReturnsOkResult()
        {
            // arrange
            int pokeId = 1;
            var expectedRating = 4.5m;

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(true);
            A.CallTo(() => _pokemonRepository.GetPokemonRating(pokeId)).Returns(expectedRating);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemonRating(pokeId);

            // assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result.As<OkObjectResult>();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(expectedRating);
        }

        [Fact]
        public void GetPokemonRating_InvalidId_ReturnsNotFound()
        {
            // arrange
            int pokeId = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(false);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemonRating(pokeId);

            // assert
            result.Should().BeOfType<NotFoundResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public void GetPokemonRating_ExceptionThrown_ReturnsInternalServerError()
        {
            // arrange
            int pokeId = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(true);
            A.CallTo(() => _pokemonRepository.GetPokemonRating(pokeId)).Throws(new Exception());

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // act
            var result = controller.GetPokemonRating(pokeId);

            // assert
            result.Should().BeOfType<ObjectResult>()
               .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void CreatePokemon_ValidInput_ReturnsNoContent()
        {
            // arrange
            int ownerId = 1;
            int categoryId = 2;
            var pokemonCreate = A.Fake<PokemonDto>();
            var pokemonMap = A.Fake<Pokemon>();

            A.CallTo(() => _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Returns(null);
            A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemonMap);
            A.CallTo(() => _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap)).Returns(true);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.CreatePokemon(ownerId, categoryId, pokemonCreate);

            // assert
            result.Should().BeOfType<NoContentResult>();
            (result as NoContentResult).StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void CreatePokemon_NullInput_ReturnsBadRequest()
        {
            // arrange
            int ownerId = 1;
            int categoryId = 2;
            PokemonDto pokemonCreate = null;

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.CreatePokemon(ownerId, categoryId, pokemonCreate);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public void CreatePokemon_ExistingPokemon_ReturnsUnprocessableEntity()
        {
            // arrange
            int ownerId = 1;
            int categoryId = 2;
            var pokemonCreate = A.Fake<PokemonDto>();
            var pokemonMap = A.Fake<Pokemon>();

            A.CallTo(() => _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Returns(pokemonMap);
            A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemonMap);
            A.CallTo(() => _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap)).Returns(false);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.CreatePokemon(ownerId, categoryId, pokemonCreate);

            // assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        }

        [Fact]
        public void CreatePokemon_FailedCreation_ReturnsInternalServerError()
        {
            // arrange
            int ownerId = 1;
            int categoryId = 2;
            var pokemonCreate = A.Fake<PokemonDto>();
            var pokemonMap = A.Fake<Pokemon>();

            A.CallTo(() => _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Returns(null);
            A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemonMap);
            A.CallTo(() => _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap)).Returns(false);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.CreatePokemon(ownerId, categoryId, pokemonCreate);

            // assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void CreatePokemon_ExceptionThrown_ReturnsInternalServerError()
        {
            // arrange
            int ownerId = 1;
            int categoryId = 2;
            var pokemonCreate = A.Fake<PokemonDto>();

            A.CallTo(() => _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Throws(new Exception());

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.CreatePokemon(ownerId, categoryId, pokemonCreate);

            // assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void DeletePokemon_ValidId_ReturnsNoContent()
        {
            // arrange
            int pokeId = 1;
            var reviews = new List<Review>();
            var pokemonToDelete = new Pokemon { Id = pokeId, Name = "Pikachu" };

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(true);
            A.CallTo(() => _reviewRepository.GetReviewsOfAPokemon(pokeId)).Returns(reviews);
            A.CallTo(() => _pokemonRepository.GetPokemon(pokeId)).Returns(pokemonToDelete);
            A.CallTo(() => _reviewRepository.DeleteReviews(reviews)).Returns(true);
            A.CallTo(() => _pokemonRepository.DeletePokemon(pokemonToDelete)).Returns(true);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.DeletePokemon(pokeId);

            // assert
            result.Should().BeOfType<NoContentResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void DeletePokemon_InvalidId_ReturnsNotFound()
        {
            // arrange
            int pokeId = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(false);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.DeletePokemon(pokeId);

            // assert
            result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public void DeletePokemon_FailedReviewDeletion_ReturnsInternalServerError()
        {
            // arrange
            int pokeId = 1;
            var reviews = new List<Review>();
            var pokemonToDelete = new Pokemon { Id = pokeId, Name = "Pikachu" };

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(true);
            A.CallTo(() => _reviewRepository.GetReviewsOfAPokemon(pokeId)).Returns(reviews);
            A.CallTo(() => _pokemonRepository.GetPokemon(pokeId)).Returns(pokemonToDelete);
            A.CallTo(() => _reviewRepository.DeleteReviews(reviews)).Returns(false);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.DeletePokemon(pokeId);

            // assert
            result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void DeletePokemon_FailedPokemonDeletion_ReturnsInternalServerError()
        {
            // arrange
            int pokeId = 1;
            var reviews = new List<Review>();
            var pokemonToDelete = new Pokemon { Id = pokeId, Name = "Pikachu" };

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Returns(true);
            A.CallTo(() => _reviewRepository.GetReviewsOfAPokemon(pokeId)).Returns(reviews);
            A.CallTo(() => _pokemonRepository.GetPokemon(pokeId)).Returns(pokemonToDelete);
            A.CallTo(() => _reviewRepository.DeleteReviews(reviews)).Returns(true);
            A.CallTo(() => _pokemonRepository.DeletePokemon(pokemonToDelete)).Returns(false);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.DeletePokemon(pokeId);

            // assert
            result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public void DeletePokemon_ExceptionThrown_ReturnsInternalServerError()
        {
            // arrange
            int pokeId = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(pokeId)).Throws(new Exception());

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            //act
            var result = controller.DeletePokemon(pokeId);

            // assert
            result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}