function dateFormatter(date) {
    if (date !== null) {
        var data = date.substring(0, 10);
        return data;
    }
}

function fullDateFormatter(date) {
    if (date !== null) {
        var data = new Date(date)
        return data.toString().substring(0,24);
    }
}

function booleanFormatter(data) {
    if (data === true)
        return "Yes";
    return "No";
}