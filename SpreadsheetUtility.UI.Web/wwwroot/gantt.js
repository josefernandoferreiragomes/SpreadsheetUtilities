//render Gantt chart for tasks
window.renderGanttTasks = (taskDataJson) => {
    const prettyJson = JSON.stringify(JSON.parse(taskDataJson), null, 2);
    console.log("Received Data: ", prettyJson); // Debug in browser console
    let tasks = JSON.parse(taskDataJson);
    document.getElementById("gantt-chart-tasks").innerHTML = ""; // Clear previous chart

    let ganttChartTaks = new Gantt("#gantt-chart-tasks", tasks, {
        on_click: (task) => console.log(task),
        on_date_change: (task, start, end) => console.log(task, start, end),
        on_progress_change: (task, progress) => console.log(task, progress),
        on_view_change: (mode) => console.log(mode),
        view_mode: 'Week',
        language: 'en',
        //holidays: { 'var(--g - weekend - highlight - color)': 'weekend' },
        lines: 'both',
        today_button: 'true',
    });

    //gantt.tasks.forEach(task => {
    //    task.resource = task.assignedTo; // Use developer name
    //});

};

//render Gantt chart for projects
window.renderGanttProjects = (taskDataJson) => {
    const prettyJson = JSON.stringify(JSON.parse(taskDataJson), null, 2);
    console.log("Received Data: ", prettyJson); // Debug in browser console
    let tasks = JSON.parse(taskDataJson);
    document.getElementById("gantt-chart-projects").innerHTML = ""; // Clear previous chart

    let ganttChartProjects = new Gantt("#gantt-chart-projects", tasks, {
        on_click: (task) => console.log(task),
        on_date_change: (task, start, end) => console.log(task, start, end),
        on_progress_change: (task, progress) => console.log(task, progress),
        on_view_change: (mode) => console.log(mode),
        view_mode: 'Week',
        language: 'en',
        //holidays: { 'var(--g - weekend - highlight - color)': 'weekend' },
        lines: 'both',
        today_button: 'true',
    });

    //gantt.tasks.forEach(task => {
    //    task.resource = task.assignedTo; // Use developer name
    //});

};
