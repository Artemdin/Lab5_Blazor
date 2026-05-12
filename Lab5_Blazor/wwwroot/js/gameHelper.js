// Функція для перерахунку координат миші у координати всередині SVG
window.gameHelper = {
    getSvgPoint: function (svgElement, clientX, clientY) {
        if (!svgElement) return { x: 0, y: 0 };

        // Створюємо точку SVG
        var pt = svgElement.createSVGPoint();
        pt.x = clientX;
        pt.y = clientY;

        // Трансформуємо координати кліка через матрицю трансформації SVG
        var svgP = pt.matrixTransform(svgElement.getScreenCTM().inverse());

        return {
            x: svgP.x,
            y: svgP.y
        };
    }
};