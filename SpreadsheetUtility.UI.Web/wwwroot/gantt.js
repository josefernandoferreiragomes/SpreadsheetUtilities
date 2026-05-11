//render Gantt chart for tasks
window.renderGanttTasks = (taskDataJson, viewMode, namedLanes) => {
    if (viewMode === undefined) {
        viewMode = 'Week';
    }
    const prettyJson = JSON.stringify(JSON.parse(taskDataJson), null, 2);
    //console.log("Received Data: ", prettyJson); // Debug in browser console
    let tasks = JSON.parse(taskDataJson);
    document.getElementById("gantt-chart-tasks").innerHTML = ""; // Clear previous chart

    //holidays are not customizable

    let ganttChartTasks = new Gantt("#gantt-chart-tasks", tasks, {
        on_click: (task) => console.log(task),
        on_date_change: (task, start, end) => console.log(task, start, end),
        on_progress_change: (task, progress) => console.log(task, progress),
        on_view_change: (mode) => console.log(mode),
        view_mode: viewMode,
        language: 'en',
        lines: 'both',
        today_button: 'true',
        view_mode_select: 'true'
    });

    if (namedLanes == 'true') {
        ganttChartTasks.tasks.forEach(task => {
            task.resource = task.assignedTo; // Use developer name
        });
    }
};

//render Gantt chart for projects
window.renderGanttProjects = (taskDataJson, viewMode, namedLanes) => {
    if (viewMode === undefined) {
        viewMode = 'Week';
    }
    const prettyJson = JSON.stringify(JSON.parse(taskDataJson), null, 2);
    //console.log("Received Data: ", prettyJson); // Debug in browser console
    let tasks = JSON.parse(taskDataJson);
    document.getElementById("gantt-chart-projects").innerHTML = ""; // Clear previous chart

    //holidays are not customizable

    let ganttChartProjects = new Gantt("#gantt-chart-projects", tasks, {
        on_click: (task) => console.log(task),
        on_date_change: (task, start, end) => console.log(task, start, end),
        on_progress_change: (task, progress) => console.log(task, progress),
        on_view_change: (mode) => console.log(mode),
        view_mode: viewMode,
        language: 'en',
        lines: 'both',
        today_button: 'true',
        view_mode_select: 'true'
    });

    if (namedLanes == 'true') {
        ganttChartProjects.tasks.forEach(task => {
            task.resource = task.assignedTo; // Use developer name
        });
    }
};

//render Gantt chart for developer tasks
window.renderDeveloperGanttTasks = (developerTaskDataJson, viewMode, namedLanes) => {
    if (viewMode === undefined) {
        viewMode = 'Week';
    }
    const prettyJson = JSON.stringify(JSON.parse(developerTaskDataJson), null, 2);
    //console.log("Received Data: ", prettyJson); // Debug in browser console
    let tasks = JSON.parse(developerTaskDataJson);
    document.getElementById("developer-gantt-chart-tasks").innerHTML = ""; // Clear previous chart

    //holidays are not customizable

    let ganttChartDeveloperTasks = new Gantt("#developer-gantt-chart-tasks", tasks, {
        on_click: (task) => console.log(task),
        on_date_change: (task, start, end) => console.log(task, start, end),
        on_progress_change: (task, progress) => console.log(task, progress),
        on_view_change: (mode) => console.log(mode),
        view_mode: viewMode,
        language: 'en',
        lines: 'both',
        today_button: 'true',
        view_mode_select: 'true'
    });

    if (namedLanes == 'true') {
        ganttChartDeveloperTasks.tasks.forEach(task => {
            task.resource = task.assignedTo; // Use developer name
        });
    }
};