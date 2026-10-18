window.BeeMartFormat = {
    rupiah: function (value) {
        var number = Math.round(Number(value) || 0);
        var text = String(Math.abs(number));
        var grouped = "";
        while (text.length > 3) {
            grouped = "." + text.slice(-3) + grouped;
            text = text.slice(0, -3);
        }
        return "Rp " + text + grouped;
    },
    percent: function (value) {
        var number = Number(value) || 0;
        return (number % 1 === 0 ? String(number) : String(number).replace(".", ",")) + "%";
    },
    weight: function (gram) {
        var value = Number(gram) || 0;
        if (value >= 1000) {
            var kg = value / 1000;
            return (kg % 1 === 0 ? String(kg) : String(kg).replace(".", ",")) + " Kg";
        }
        return value + " g";
    }
};
