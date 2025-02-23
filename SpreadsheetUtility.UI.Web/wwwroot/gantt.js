window.renderGantt = (taskDataJson) => {
    console.log("Received Data: ", taskDataJson); // Debug in browser console
    let tasks = JSON.parse(taskDataJson);
    document.getElementById("gantt-chart").innerHTML = ""; // Clear previous chart

    let gantt = new Gantt("#gantt-chart", tasks, {
        on_click: (task) => console.log(task),
        on_date_change: (task, start, end) => console.log(task, start, end),
        on_progress_change: (task, progress) => console.log(task, progress),
        on_view_change: (mode) => console.log(mode),
        view_mode: 'Week',
        language: 'en'
    });
};
