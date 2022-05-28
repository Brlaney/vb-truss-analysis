Imports BriefFiniteElementNet
Imports BriefFiniteElementNet.Elements

Module Program
    Sub Main()
        Dim Model As New Model()
        Dim Load As New NodalLoad()
        Dim force As New Force()

        'Node Coordinates are in inches (US Imperial)
        Dim n1 As New Node(0, 0, 0)
        Dim n2 As New Node(120, 120, 0)
        Dim n3 As New Node(240, 120, 0)
        Dim n4 As New Node(120, 0, 0)

        n1.Constraints = Constraints.MovementFixed 'Pin support
        n2.Constraints = Constraints.RotationFixed 'Joint connection
        n3.Constraints = Constraints.RotationFixed 'Joint connection
        n4.Constraints = Constraints.MovementFixed 'Pin support

        Dim elm1 = New BarElement(n1, n2)
        Dim elm2 = New BarElement(n2, n3)
        Dim elm3 = New BarElement(n3, n4)
        Dim elm4 = New BarElement(n1, n3)
        Dim elm5 = New BarElement(n2, n4)

        elm1.Behavior = elm2.Behavior = elm3.Behavior = elm4.Behavior = elm5.Behavior = BarElementBehaviour.Truss

        'Given:
        'A = 2.0 in^2
        'E = 29 * 10^6 psi
        'Assume: Poisson's ratio, e = 0.33

        elm1.Section = New Sections.UniformParametric1DSection(2.0)
        elm2.Section = New Sections.UniformParametric1DSection(2.0)
        elm3.Section = New Sections.UniformParametric1DSection(2.0)
        elm4.Section = New Sections.UniformParametric1DSection(2.0)
        elm5.Section = New Sections.UniformParametric1DSection(2.0)
        elm1.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(29000000.0, 0.33)
        elm2.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(29000000.0, 0.33)
        elm3.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(29000000.0, 0.33)
        elm4.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(29000000.0, 0.33)
        elm5.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(29000000.0, 0.33)

        Model.Nodes.Add(n1, n2, n3, n4)
        Model.Elements.Add(elm1, elm2, elm3, elm4, elm5)

        'frc.Fy = -30000 '30,000 lbs of force applied downward (-) onto Node 2.
        force.Fy = -30 '30 kips of force applied downward (-) onto Node 2.
        Load.Force = force

        n2.Loads.Add(Load)
        Model.Solve_MPC()

        Dim d2 = n2.GetNodalDisplacement()
        Dim d3 = n3.GetNodalDisplacement()

        Dim d_21 = Math.Round(d2.DX, 6).ToString
        Dim d_22 = Math.Round(d2.DY, 6).ToString

        Dim d_31 = Math.Round(d3.DX, 6).ToString
        Dim d_32 = Math.Round(d3.DY, 6).ToString

        Dim OutputString As New Text.StringBuilder
        With OutputString
            .AppendLine(vbCrLf & $"Node 2 displacements: ")
            .AppendLine($"Along the x-axis {d_21} in")
            .AppendLine($"Along the y-axis {d_22} in")
            .AppendLine(vbCrLf & $"Node 3 displacements: ")
            .AppendLine($"Along the x-axis {d_31} in")
            .AppendLine($"Along the y-axis {d_32} in")
        End With

        Console.WriteLine(OutputString.ToString)
    End Sub
End Module
