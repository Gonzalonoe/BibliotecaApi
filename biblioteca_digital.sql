-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 09-11-2025 a las 18:48:27
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `biblioteca_digital`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `libros`
--

CREATE TABLE `libros` (
  `Id` int(11) NOT NULL,
  `Titulo` varchar(200) NOT NULL,
  `Autor` varchar(200) NOT NULL,
  `Anio` int(11) DEFAULT NULL,
  `Stock` int(11) NOT NULL DEFAULT 0,
  `Descripcion` text DEFAULT NULL,
  `portada` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `libros`
--

INSERT INTO `libros` (`Id`, `Titulo`, `Autor`, `Anio`, `Stock`, `Descripcion`, `portada`) VALUES
(2, 'Juego de Tronos', 'George R.R. Martin', 1996, 10, '\"Juego de Tronos\" es una serie de fantasía medieval que trata sobre la lucha de varias familias nobles por el control del continente de Poniente y el Trono de Hierro, todo ello ambientado en un mundo ficticio con elementos fantásticos como dragones y otros seres sobrenaturales. La trama entrelaza batallas políticas, intrigas, traiciones y un conflicto en el norte de Poniente con una amenaza de muertos vivientes conocida como los caminantes blancos. La historia se basa en la saga de novelas \"Canción de Hielo y Fuego\" de George R. R. Martin.  ', 'http://127.0.0.1:5000/portadas/b1dfa676-827c-48e5-9524-5078f78029f5.jpeg'),
(5, 'El Hobbit', 'J.R.R. Tolkien', 1937, 11, 'El Hobbit trata sobre la aventura de Bilbo Bolsón, un hobbit hogareño que se une al mago Gandalf y a trece enanos liderados por Thorin Escudo de Roble en una misión para recuperar la Montaña Solitaria y su tesoro del dragón Smaug. Durante su viaje, Bilbo descubre su propio valor y coraje, vive peligros en tierras salvajes y misteriosas, y encuentra un anillo mágico que cambia su vida para siempre. ', 'http://127.0.0.1:5000/portadas/2e7a1822-4224-480d-9807-1fdbd71812c1.jpg'),
(7, 'Harry Potter', 'J. K. Rowling', 1990, 10, 'Harry Potter es la historia de un huérfano que descubre en su undécimo cumpleaños que es un mago y asiste al Colegio Hogwarts de Magia y Hechicería. Allí, hace amigos como Ron y Hermione y descubre su conexión con el mundo mágico. La trama se centra en su lucha contra el malvado mago Lord Voldemort, quien mató a sus padres cuando Harry era bebé', 'http://127.0.0.1:5000/portadas/0e80dba7-6966-4a38-a0d0-4615fa843a2d.jpg'),
(8, 'El Mago de Oz', 'Lyman Frank Baum', 1900, 5, 'l maravilloso mago de Oz de Lyman Frank Baum narra la aventura de la joven Dorothy Gale y su perro Totó, quienes son transportados por un ciclón desde su hogar en la granja de Kansas a la mágica y colorida tierra de Oz. Su casa cae sobre la Malvada Bruja del Este, matándola y liberando así a los Munchkins de su esclavitud. Para volver a casa, Dorothy debe viajar a la Ciudad Esmeralda y pedir ayuda al poderoso Mago de Oz.', 'http://127.0.0.1:5000/portadas/d947a36d-0a08-4524-87db-069286ee5cd3.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pedidos`
--

CREATE TABLE `pedidos` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL,
  `LibroId` int(11) DEFAULT NULL,
  `TituloSolicitado` varchar(250) DEFAULT NULL,
  `FechaPedido` datetime NOT NULL,
  `FechaVencimiento` datetime DEFAULT NULL,
  `FechaDevolucion` datetime DEFAULT NULL,
  `Estado` int(11) NOT NULL,
  `Observaciones` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pedidos`
--

INSERT INTO `pedidos` (`Id`, `UsuarioId`, `LibroId`, `TituloSolicitado`, `FechaPedido`, `FechaVencimiento`, `FechaDevolucion`, `Estado`, `Observaciones`) VALUES
(9, 1, 5, 'El Hobbit', '2025-11-09 04:21:52', '2025-11-16 04:21:52', '2025-11-09 04:25:51', 5, 'Libro devuelto correctamente.'),
(10, 1, 8, 'El Mago de Oz', '2025-11-09 13:45:15', '2025-11-16 13:45:15', NULL, 0, 'Pendiente de entrega'),
(11, 1, 7, 'Harry Potter', '2025-11-09 13:45:18', '2025-11-16 13:45:18', '2025-11-09 17:02:26', 3, 'Libro devuelto correctamente.'),
(12, 1, 2, 'Juego de Tronos', '2025-11-09 13:45:21', '2025-11-16 13:45:21', '2025-11-09 13:46:03', 5, 'Libro devuelto correctamente.');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reportes`
--

CREATE TABLE `reportes` (
  `Id` int(11) NOT NULL,
  `Fecha` datetime NOT NULL DEFAULT current_timestamp(),
  `TituloLibro` varchar(100) NOT NULL,
  `Sinopsis` varchar(500) DEFAULT NULL,
  `ImagenPortada` varchar(255) DEFAULT NULL,
  `UsuarioId` int(11) NOT NULL,
  `UsuarioNombre` varchar(100) NOT NULL,
  `UsuarioEmail` varchar(150) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `reportes`
--

INSERT INTO `reportes` (`Id`, `Fecha`, `TituloLibro`, `Sinopsis`, `ImagenPortada`, `UsuarioId`, `UsuarioNombre`, `UsuarioEmail`) VALUES
(1, '2025-11-08 19:37:39', 'El Mago de OZ', 'Dorothy, que sueña con viajar más allá del arco iris, ve su deseo hecho realidad cuando un tornado se la lleva con su perrito al mundo de Oz. Entonces se dirige por el Camino Amarillo hacia la Ciudad Esmeralda, donde vive el todopoderoso Mago de Oz, que puede ayudarla a regresar a Kansas. Durante el viaje, se hace amiga del Espantapájaros que desea un cerebro, el Hombre de Hojalata que quiere un corazón y el León Cobarde le hace falta el coraje, por lo que deciden unirse a Dorothy en su odisea.', NULL, 1, 'Admin', 'admin@demo.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(120) NOT NULL,
  `Email` varchar(180) NOT NULL,
  `Rol` int(11) NOT NULL,
  `PasswordHash` varchar(200) NOT NULL,
  `PasswordSalt` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Nombre`, `Email`, `Rol`, `PasswordHash`, `PasswordSalt`) VALUES
(1, 'Admin', 'admin@demo.com', 1, 'SxBJUqEODjrOScMn//skuL573qzk32WKHjI7dbG9K7I=', 'xh/yCEzIpnRHAHo3fdcCaQ=='),
(3, 'gonza', 'gonza@gmail.com', 2, '7lVbRfHHYFQmkOg4V0FYOI9LPG0mtLw3Y10cT+vIPxc=', '6SiSpJdkZydExe8Fxd5iGA==');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `libros`
--
ALTER TABLE `libros`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `pedidos`
--
ALTER TABLE `pedidos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_pedidos_usuario` (`UsuarioId`),
  ADD KEY `fk_pedidos_libro` (`LibroId`);

--
-- Indices de la tabla `reportes`
--
ALTER TABLE `reportes`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_Reportes_Usuarios` (`UsuarioId`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `libros`
--
ALTER TABLE `libros`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `pedidos`
--
ALTER TABLE `pedidos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `reportes`
--
ALTER TABLE `reportes`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `pedidos`
--
ALTER TABLE `pedidos`
  ADD CONSTRAINT `fk_pedidos_libro` FOREIGN KEY (`LibroId`) REFERENCES `libros` (`Id`) ON DELETE SET NULL,
  ADD CONSTRAINT `fk_pedidos_usuario` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`);

--
-- Filtros para la tabla `reportes`
--
ALTER TABLE `reportes`
  ADD CONSTRAINT `FK_Reportes_Usuarios` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
